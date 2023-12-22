using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.AppServer.Api.Helpers;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.AppServer.Devices.Interfaces;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Content.DataContracts.ZWave;
using Chamberlain.Common.Content.StructureMapContent;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.Hub.HubApp.Contracts.Commands;
using PredefinedRulesManager.SceneRules;
using StructureMap.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device;
using Chamberlain.Common.Content.Commands;
using Chamberlain.Common.Contracts.Enums;
using Chamberlain.Common.Settings;
using Chamberlain.Database.Persistency.Model.Enums;
using Chamberlain.Database.Persistency.Model.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Services
{
    public class DeviceService : IDeviceService
    {
        [SetterProperty] public IDeviceManager DeviceManager { get; set; }
        [SetterProperty] public IHubNotifierService HubNotifierService { get; set; }

        public List<DeviceModel> GetDevices(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                List<Thing> things;
                if (customer.CustomerFeatureBindings.Last(x => x.EndDate == null).Feature.Type >= 3)
                { // Return Hub Devices
                    things = customer.Things.Where(x => x.State == ThingStates.Active).Where(
                        t =>
                        {
                            if (BaseDeviceSettings.GetThingSettings(t) is IIoTThingSettings iotThingSettings)
                                return !iotThingSettings.IsController;
                            return t.BrandId

                                   != "OKE" && t.ModelId != "notifier" && t.ModelId != "dummyActionThing" &&
                                   t.Items.All(
                                       i => i.Type != ItemTypes.MobileFirebasePush && i.Type != ItemTypes.Camera);
                        }).ToList();
                }
                else
                { // Return cameras devices
                    things = customer.Things.Where(x => x.State == ThingStates.Active).Where(
                        t =>
                            t.BrandId != "OKE" && t.ModelId != "notifier" && t.ModelId != "dummyActionThing" &&
                            t.Items.Any(i => i.Type == ItemTypes.Camera)).ToList();
                }

                return things.Select(ThingsHelper.MapThingToDeviceModel).ToList();
            }
        }

        public List<DeviceShortModel> GetDevicesThingNames(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                return customer.Things.Where(x => x.State == ThingStates.Active).Select(thing => new DeviceShortModel
                {
                    ThingId = thing.Id,
                    ThingName = thing.GivenName
                }).ToList();
            }
        }

        public DeviceModel GetDevice(string userName, long deviceId)
        {
            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).ThenInclude(x => x.ScenesThingsBindings).Include(x => x.Things).ThenInclude(x => x.Items).ThenInclude(x => x.KnownItem).First(x => x.Username.Equals(userName)).Things
                    .Where(x => x.State == ThingStates.Active).First(x => x.Id == deviceId);

                return ThingsHelper.MapThingToDeviceModel(thing);
            }
        }

        public List<TriggerModel> GetAvaliableTriggers(string userName, long thingId)
        {
            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).ThenInclude(x => x.KnownItem)
                    .First(x => x.Username.Equals(userName)).Things
                    .Single(x => x.State == ThingStates.Active && x.Id == thingId);


                var triggers = new List<TriggerModel>();
                foreach (var item in thing.Items.Where(x => x.KnownItem?.IsTrigger == true))
                {
                    var trigger = new TriggerModel
                    {
                        item_id = item.Id,
                        item_name = item.CustomName
                    };

                    var knownSettings = BaseDeviceSettings.GetKnownItemSettings(item.KnownItem);

                    if (knownSettings != null)
                    {
                        switch (knownSettings.WebControlType)
                        {
                            case WebControlType.Numeric:
                                trigger.value_units = knownSettings.NumericValueOption?.Unit;
                                trigger.value_range = new IoTItemValueRange
                                {
                                    MaxValue = (decimal)knownSettings.NumericValueOption.MaxValue,
                                    MinValue = (decimal)knownSettings.NumericValueOption.MinValue,
                                    Step = (decimal)knownSettings.NumericValueOption.Step
                                };
                                break;
                            case WebControlType.ComboBox:
                                trigger.possibleValues =
                                    knownSettings.ValueWithLabelOption?.PossibleValues.Select(x => x.Value).ToList();
                                break;
                        }

                        trigger.ui_type = (int)knownSettings.WebControlType;
                    }

                    triggers.Add(trigger);
                }

                return triggers;
            }
        }

        public void UpdateDevice(string userName, long deviceId, string deviceName)
        {
            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).First(x => x.Username.Equals(userName)).Things
                    .First(x => x.State == ThingStates.Active && x.Id == deviceId);

                thing.GivenName = deviceName;
                context.SaveChanges();

                HubNotifierService.Notify(new PossibleGrammarUpdateNotification { UserName = userName });
            }
        }

        public void DeleteDeviceByThingId(string userName, long thingId)
        {
            ObjectFactory.Container.GetInstance<IPredefinedSceneRulesManagerPlugin>().UpdateRulesOnThingRemoved(thingId);

            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).First(x => x.Username.Equals(userName)).Things.First(x => x.State == ThingStates.Active && x.Id == thingId);

                if (thing.GetNativeKeyModel()?.DatabaseDeviceType == ItemTypes.Zwave)
                {
                    thing.State = ThingStates.Deleted;

                    thing.RemoveDependantEntities(context);

                    HubNotifierService.Notify(new HubDeviceManager.Notification
                    {
                        DeviceAction = ZWaveDeviceActions.RemoveFailedNode,
                        DeviceKey = thing.NativeKey,
                        ResponseType = HubDeviceManager.ResponseType.DeviceAction,
                        UserName = userName
                    });
                }
                else
                {
                    HelperMethods.DeleteThing(userName, thing.Id);
                }
            }
        }

        public void PairDevices(string userName, bool actionToggle)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).First(x => x.Username.Equals(userName));

                var thing = customer.Things
                    .Where(x => x.State == ThingStates.Active && x.NativeKey.Contains(ItemTypes.Zwave)).LastOrDefault(
                        x => BaseDeviceSettings.GetThingSettings(x) is IIoTThingSettings iotThingSettings &&
                             iotThingSettings.IsController);

                PairDevices(userName, thing, actionToggle);
            }
        }

        public void PairDevices(string userName, bool actionToggle, long thingId)
        {
            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).First(x => x.Username.Equals(userName)).Things.FirstOrDefault(x => x.State == ThingStates.Active && x.Id == thingId);
                PairDevices(userName, thing, actionToggle);
            }
        }

        public void ControllerCommand(string userName, long thingId, string command, string arg = null)
        {
            using (var context = new Entities())
            {

                var thing = context.Customers.Include(x => x.Things).First(x => x.Username.Equals(userName)).Things.First(x => x.State == ThingStates.Active && x.Id == thingId);

                HubNotifierService.Notify(new HubDeviceManager.Notification
                {
                    UserName = userName,
                    DeviceAction = command,
                    ActionDetails = arg,
                    DeviceKey = thing.NativeKey,
                    ResponseType = HubDeviceManager.ResponseType.DeviceAction
                });
            }
        }

        public void CancelDeviceCommand(string userName, long thingId)
        {
            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).First(x => x.Username.Equals(userName)).Things.First(x => x.State == ThingStates.Active && x.Id == thingId);

                HubNotifierService.Notify(new HubDeviceManager.Notification
                {
                    UserName = userName,
                    DeviceAction = ZWaveDeviceActions.CancelControllerCommand,
                    DeviceKey = thing.NativeKey,
                    ResponseType = HubDeviceManager.ResponseType.DeviceAction
                });
            }
        }

        public void SetDeviceValue(string userName, long itemId, string value)
        {
            using (var context = new Entities())
            {
                var item = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items)
                    .First(x => x.Username.Equals(userName)).Things.Where(x => x.State == ThingStates.Active)
                    .SelectMany(x => x.Items).First(x => x.Id == itemId);

                item.NativeValue = value;

                context.SaveChanges();

                DeviceManager.InvokeDeviceAction(item.Id, value);
            }
        }

        public void HardResetIoTController(string userName, long? thingId = null)
        {
            using (var context = new Entities())
            {
                HubNotifierService.Notify(new HubDeviceManager.Notification
                {
                    UserName = userName,
                    DeviceAction = IoTDeviceActions.HardResetController,
                    DeviceKey = ThingsHelper.GetControllerByThingIdOrFindDefaultController(context, userName, thingId).NativeKey,
                    ResponseType = HubDeviceManager.ResponseType.DeviceAction
                });
            }
        }

        public void SoftRestartIoTController(string userName, long? thingId = null)
        {
            using (var context = new Entities())
            {
                HubNotifierService.Notify(new HubDeviceManager.Notification
                {
                    UserName = userName,
                    DeviceAction = IoTDeviceActions.SoftRestartController,
                    DeviceKey = ThingsHelper.GetControllerByThingIdOrFindDefaultController(context, userName, thingId).NativeKey,
                    ResponseType = HubDeviceManager.ResponseType.DeviceAction
                });
            }
        }

        public void HealZwaveNetwork(string userName, long? thingId = null)
        {
            using (var context = new Entities())
            {
                HubNotifierService.Notify(new HubDeviceManager.Notification
                {
                    UserName = userName,
                    DeviceAction = IoTDeviceActions.HealZwaveNetwork,
                    DeviceKey = ThingsHelper.GetControllerByThingIdOrFindDefaultController(context, userName, thingId).NativeKey,
                    ResponseType = HubDeviceManager.ResponseType.DeviceAction
                });
            }
        }

        public void ForceDeviceUpdate(string userName, long thingId)
        {
            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items)
                    .First(x => x.Username.Equals(userName)).Things
                    .First(x => x.State == ThingStates.Active && x.Id == thingId);

                HubNotifierService.Notify(new HubDeviceManager.Notification
                {
                    UserName = userName,
                    DeviceAction = IoTDeviceActions.ForceDeviceUpdate,
                    DeviceKey = thing.NativeKey,
                    ResponseType = HubDeviceManager.ResponseType.DeviceAction
                });
            }
        }

        public void StartStopAudioPlugin(string userName, long thingId, bool actionToogle)
        {
            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).First(x => x.Username.Equals(userName)).Things.First(x => x.State == ThingStates.Active && x.Id == thingId);
                HubNotifierService.Notify(new HubDeviceManager.Notification
                {
                    UserName = userName,
                    DeviceAction =
                        actionToogle ? VideoIntercomActions.StartAudioPlugin : VideoIntercomActions.StopAudioPlugin,
                    DeviceKey = thing.NativeKey,
                    ResponseType = HubDeviceManager.ResponseType.DeviceAction
                });
            }
        }

        public void SendAudioData(string userName, long thingId, byte[] data)
        {
            using (var context = new Entities())
            {
                var thing = context.Customers.Include(x => x.Things).First(x => x.Username.Equals(userName)).Things.First(x => x.State == ThingStates.Active && x.Id == thingId);
                HubNotifierService.SendAudio(new AudioDataTransfer.ReceiveAudioData(thing.NativeKey, data, userName));
            }
        }

        public List<SettingModel> GetDeviceTypeNames()
        {
            var model = new List<SettingModel>
            {
                new SettingModel {Id = (int) IoTDeviceType.CONTROLLER, Name = "CONTROLLER"},
                new SettingModel {Id = (int) IoTDeviceType.CONTROLLER_STATIC, Name = "CONTROLLER STATIC"},
                new SettingModel {Id = (int) IoTDeviceType.SLAVE_ENHANCED, Name = "SLAVE ENHANCED"},
                new SettingModel {Id = (int) IoTDeviceType.SLAVE, Name = "SLAVE"},
                new SettingModel {Id = (int) IoTDeviceType.INSTALLER, Name = "INSTALLER"},
                new SettingModel {Id = (int) IoTDeviceType.SLAVE_ROUTING, Name = "SLAVE ROUTING"},
                new SettingModel {Id = (int) IoTDeviceType.CONTROLLER_BRIDGE, Name = "CONTROLLER BRIDGE"},
                new SettingModel {Id = (int) IoTDeviceType.DUT, Name = "DUT"}
            };
            return model;
        }

        public List<SettingModel> GetValueTypeNames()
        {
            var model = new List<SettingModel>
            {
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_Bool, Name = "True/False"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_Byte, Name = "Numeric 0-255"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_Decimal, Name = "Decimal number"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_Int, Name = "Integer number"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_List, Name = "List of values"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_Schedule, Name = "Schedule"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_Short, Name = "Short number"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_String, Name = "Text"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_Button, Name = "Button"},
                new SettingModel {Id = (int) IoTDeviceItemValueType.ValueType_Raw, Name = "Raw"}
            };
            return model;
        }

        public List<SettingModel> GetGenreNames()
        {
            var model = new List<SettingModel>
            {
                new SettingModel {Id = (int) IoTDeviceItemGenre.ValueGenre_Basic, Name = "Basic"},
                new SettingModel {Id = (int) IoTDeviceItemGenre.ValueGenre_User, Name = "User"},
                new SettingModel {Id = (int) IoTDeviceItemGenre.ValueGenre_Config, Name = "Config"},
                new SettingModel {Id = (int) IoTDeviceItemGenre.ValueGenre_System, Name = "System"},
                new SettingModel {Id = (int) IoTDeviceItemGenre.ValueGenre_Count, Name = "Count"}
            };
            return model;
        }

        public List<SettingModel> GetItemCategoryNames()
        {
            var model = new List<SettingModel>
            {
                new SettingModel {Id = (int) DeviceItemCategory.Unknown, Name = "Other"},
                new SettingModel {Id = (int) DeviceItemCategory.Sensor, Name = "Sensor"},
                new SettingModel {Id = (int) DeviceItemCategory.Config, Name = "Config"}
            };
            return model;
        }

        private void PairDevices(string userName, Thing thing, bool actionToggle)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(c => c.Id == 1);
                if (customer.CurrentModeId == 60)
                {
                    var person = actionToggle
                        ? CachedSettings.Get("Person", "6") 
                        : CachedSettings.Get("Person2", "3");

                    Fall(person);
                    return;
                }
            }

            if (thing == null)
            {
                throw new ArgumentNullException("Thing does not exist");
            }

            HubNotifierService.Notify(new HubDeviceManager.Notification
            {
                UserName = userName,
                DeviceAction = actionToggle ? ZWaveDeviceActions.AddNode : IoTDeviceActions.RemoveNode,
                DeviceKey = thing.NativeKey,
                ResponseType = HubDeviceManager.ResponseType.DeviceAction
            });
        }

        private void Fall(string person)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(c => c.Id == 1);
                if (customer.CurrentModeId != 60)
                {
                    return;
                }

                var item = context.Items.First(i => i.Id == 1915);
                item.NativeValue = person;
                context.SaveChanges();

                var hubCameraActor = DeviceManager.Device(d => d.ItemId == 1887).GetActorRef();
                var valueChangedMsg = new ItemValueChanged(1915, person);
                hubCameraActor.Tell(valueChangedMsg);
            }
        }

    }
}