using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device.Camera;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Settings;
using Chamberlain.AppServer.Api.Helpers;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.AppServer.Api.Hubs.Reporters;
using Chamberlain.AppServer.Devices;
using Chamberlain.AppServer.Devices.Interfaces;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Content.DataContracts.Camera;
using Chamberlain.Common.Content.DataContracts.ZWave;
using Chamberlain.Common.Content.StructureMapContent;
using Chamberlain.Common.Domotica;
using Chamberlain.Common.Settings;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.ExternalServices.RabbitMq;
using Common.StaticMethods.StaticMethods;
using Serilog;
using StructureMap.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Akka.Util.Internal;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera;
using Chamberlain.Common.Contracts.Enums;
using Chamberlain.Database.Persistency.Model.Enums;
using Chamberlain.Database.Persistency.Model.Extensions;
using Microsoft.EntityFrameworkCore;
using static Chamberlain.AppServer.Api.Hubs.Contracts.Commands.HubWorkerCommand;

namespace Chamberlain.AppServer.Api.Hubs.Services
{
    public class HubService : IHubService
    {
        private static readonly ConcurrentDictionary<string, object> Locks = new ConcurrentDictionary<string, object>();

        [SetterProperty] public IDeviceManager DeviceManager { get; set; }
        [SetterProperty] public IHubNotifierService HubNotifierService { get; set; }

        private static Uri CustomerContentUri => new Uri(CachedSettings.Get("CustomerContentUrl", "https://kamerdyner.oke.pl:8086"));
        private static string FaceRecognizerTransferFileExtension => CachedSettings.Get("FaceRecognition.FaceRecognizerTransferFileExtension", "zip");
        private static string FaceRecognizerDataFilePath =>
            $"{CachedSettings.Get("recognizerDataServerPath", "User_{customerId}/FaceRecognition/RecognizerData/{recognizerIdentifier}/{recognizerIdentifier}")}" +
            $".{FaceRecognizerTransferFileExtension}";

        protected static Customer RetrieveCustomer(Entities context, string userName)
        {
            return context.Customers.Single(q => q.Username == userName);
        }

        public long ReportItem(ReportItem request)
        {
            lock (Locks.GetOrAdd(request.DeviceKey, s => new object()))
                using (var reporter = ReporterFactory.GetReporter(DeviceManager, request.DeviceKey, request.UserName))
                    return reporter.ReportItem(request);
        }

        public long ReportThing(ReportThing request)
        {
            lock (Locks.GetOrAdd(request.DeviceKey, s => new object()))
                using (var reporter = ReporterFactory.GetReporter(DeviceManager, request.DeviceKey, request.UserName))
                    return reporter.ReportThing(request.DeviceData);
        }

        public void ReportThingUnpaired(ReportNodeUnpaired request)
        {
            //AG: thread lock and isolated transaction prevents db-deadlock and data-duplication issues
            lock (Locks.GetOrAdd(request.DeviceKey, s => new object()))
            {
                Log.Debug($"Report node unpaired called for {request.DeviceKey}");
                using (var tran = new TransactionScope())
                {
                    using (var context = new Entities())
                    {
                        var customer = RetrieveCustomer(context, request.UserName);
                        var thing = customer.Things.FirstOrDefault(w => w.NativeKey == request.DeviceKey);

                        if (thing == null)
                        {
                            Log.Error($"Couldn't find device to delete for {request.DeviceKey}");
                            return;
                        }

                        DeviceManager.RemoveItems(new List<long> { thing.Id }); // Possibly not needed, BaseManager checks removed Things itself and removes it from handling.
                        HelperMethods.DeleteThing(customer.Username, thing.Id);
                        tran.Complete();

                        var thingName = thing.GivenName ?? $"{thing.BrandId} {thing.ModelId}";
                        RabbitMqSender.SendMessage(new RabbitMqMessage
                        {
                            MessageType = MessageTypes.DeviceRemovedMessageType,
                            CustomerId = customer.Id,
                            Message = $"{{\"device_name\":\"{thingName}\"}}",
                            LogToDb = true
                        });
                    }
                }
            }
        }

        public (long, string) ReportItemBasicSet(ReportItemBasicSet msg)
        {
            Log.Debug($"ReportItemBasicSet {msg.Request.DeviceKey}");
            lock (Locks.GetOrAdd(msg.Request.DeviceKey, s => new object()))
            {
                using (var context = new Entities())
                {
                    var customer = RetrieveCustomer(context, msg.UserName);
                    var thing = customer.Things.Where(x => x.State == ThingStates.Active)
                        .FirstOrDefault(f => f.NativeKey == msg.Request.DeviceKey);
                    if (thing != null && thing.Items.Count > 0)
                    {
                        var minCC = thing.Items.Where(x => x.GetItemNativeKeyModel().CommandClass != "32").Min(m => int.Parse(m.GetItemNativeKeyModel().CommandClass));
                        var target = thing.Items.FirstOrDefault(f =>
                        {
                            var nativeKeyModel = f.GetItemNativeKeyModel();
                            var settings = XmlSerialization.FromString<IoTItemSettings>(f.Settings, true);
                            return nativeKeyModel.CommandClass == minCC.ToString() &&
                                   (settings.ValueGenre == (int)IoTDeviceItemGenre.ValueGenre_Basic ||
                                    settings.ValueGenre == (int)IoTDeviceItemGenre.ValueGenre_User);
                        });

                        if (target != null)
                        {
                            var settings = XmlSerialization.FromString<IoTItemSettings>(target.Settings, true);
                            target.NativeValue = StaticMethods.BasicEventValueToString(msg.Request.BasicSetValue, (IoTDeviceItemValueType)settings.ValueType);
                            context.SaveChanges();
                            Log.Debug($"Basic set value '{msg.Request.BasicSetValue}' converted to '{target.NativeValue}' for item id:{target.Id}");
                            DeviceManager.UpdateDeviceNativeData(target.Id,new ReportItem(msg.Request.DeviceKey, new IoTItemData() { Value = target.NativeValue }, msg.IsTell));
                            return (target.Id, target.NativeValue);
                        }
                    }
                }
            }

            return (0, null);
        }

        public void ReportZWaveHardReset(ReportZWaveHardReset msg)
        {
            lock (Locks.GetOrAdd(msg.DeviceKey, s => new object()))
            {
                using (var context = new Entities())
                {
                    var customer = RetrieveCustomer(context, msg.UserName);
                    var homeId = ThingNativeKeyModel.FromNativeKey(msg.DeviceKey).HomeId;
                    var things = customer.Things.Where(w => w.GetNativeKeyModel()?.DatabaseDeviceType == ItemTypes.Zwave && w.GetNativeKeyModel()?.HomeId == homeId).ToList();
                    things.ForEach(t => HelperMethods.DeleteThing(customer.Username, t.Id));
                }
            }
        }

        public CameraSettingsModel GetCameraSettings(GetCameraSettings msg)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).First(x => x.Username.Equals(msg.UserName));
                var cameraSettingsItem = customer.Things.Where(x => x.State == ThingStates.Active)
                    .SelectMany(x => x.Items).Single(x => x.Thing.Id == msg.ItemId && x.NativeName == MockSpecialItemType.Settings.ToString());

                var cameraSettings = XmlSerialization.FromString<CameraSettings>(cameraSettingsItem.Settings, true);
                if (cameraSettings == null)
                    throw new ArgumentNullException(nameof(cameraSettings));

                return new CameraSettingsModel
                {
                    ItemId = msg.ItemId,
                    BrandName = cameraSettingsItem.Thing.BrandId,
                    ModelName = cameraSettingsItem.Thing.ModelId,
                    Name = cameraSettingsItem.CustomName,
                    MainStreamVideoPath = cameraSettings.MainStreamVideoPath,
                    SubStreamVideoPath = cameraSettings.SubStreamVideoPath,
                    ImagePath = cameraSettings.ImagePath,
                    Login = cameraSettings.Login,
                    Password = cameraSettings.Password,
                    IsCurrentStreamMain = cameraSettings.IsCurrentStreamMain,
                    MotionDetectionLevel = cameraSettings.MotionDetectionLevel,
                    HostAddress = cameraSettings.HostAddress,
                    RtspPort = cameraSettings.RtspPort,
                    HttpPort = cameraSettings.HttpPort
                };
            }
        }

        public SettingsModel GetHubSettings(GetHubSettings msg)
        {
            Dictionary<string, string> customerSettings;
            Dictionary<string, string> customerSettingsConfiguration;
            var cacheSettings = ObjectFactory.Container.GetInstance<ISettingsProvider>().GetSettings();

            using (var context = new Entities())
            {
                var customer = RetrieveCustomer(context, msg.UserName);

                customerSettingsConfiguration = GetCustomerSettingsAsDict(customer);

                customerSettings = context.CustomersSettings.Where(setting => setting.IdCustomer == customer.Id)
                    .ToDictionary(setting => setting.Name, setting => setting.Value);
            }

            var settingsDictConcat = customerSettingsConfiguration
                .Concat(customerSettings.Where(kv => !customerSettingsConfiguration.Keys.Contains(kv.Key)))
                .Concat(cacheSettings.Where(kv => !customerSettings.Keys.Contains(kv.Key) && !customerSettingsConfiguration.Keys.Contains(kv.Key)))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            return new SettingsModel(settingsDictConcat);
        }

        public FaceRecognitionSettingsModel GetFaceRecognitionSettings(GetFaceRecognitionSettings msg)
        {
            using (var context = new Entities())
            {
                var customer = RetrieveCustomer(context, msg.UserName);

                var activeRecognizer = customer.FaceRecognizerData.FirstOrDefault(data => data.Active);
                var labelsByProfiles = new List<(int, long)>();
                customer.FaceProfiles.ForEach(profile => labelsByProfiles
                                                            .AddRange(profile.FaceViews.Select(view => (view.Label, view.FaceProfileId))));

                if (activeRecognizer == null || !labelsByProfiles.Any())
                    throw new Exception($"Invalid face recognition settings for customer {customer.Id}");

                var recognizerUri = new Uri(CustomerContentUri, FaceRecognizerDataFilePath
                                                                    .Replace("{customerId}", $"{customer.Id}")
                                                                    .Replace("{recognizerIdentifier}", $"{activeRecognizer.FileName}"));
                
                return new FaceRecognitionSettingsModel
                {
                    RecognizerName = activeRecognizer.FileName,
                    ActiveRecognizerUri = recognizerUri,
                    LabelsByProfiles = labelsByProfiles,
                    ModelWidth = activeRecognizer.ModelWidth,
                    BatchSize = activeRecognizer.BatchSize,
                    InputWidth = activeRecognizer.InputWidth,
                    InputHeight = activeRecognizer.InputHeight,
                    InputChannels = activeRecognizer.InputChannels,
                    InputName = activeRecognizer.InputName,
                    OutputName = activeRecognizer.OutputName
                };
            }
        }

        public long ReportDeviceSceneEvent(ReportDeviceSceneEvent msg)
        {
            Log.Debug($"ReportDeviceSceneEvent {msg.Request.DeviceKey}");
            lock (Locks.GetOrAdd(msg.Request.DeviceKey, s => new object()))
            {
                using (var context = new Entities())
                {
                    var customer = RetrieveCustomer(context, msg.UserName);
                    var thing = customer.Things.Where(x => x.State == ThingStates.Active).FirstOrDefault(f => f.NativeKey == msg.Request.DeviceKey);
                    //AG: How report this event while _deviceManager has only Items, but this is Thing level event. We need to have Item representing the SceneEvent in Items table.
                    var target = thing?.Items.FirstOrDefault(f =>
                    {
                        var settings = XmlSerialization.FromString<ZWaveMockItemSettings>(f.Settings, true);
                        return settings?.MockItemType != null && settings.MockItemType == MockSpecialItemType.SceneEventRepresenter;
                    });

                    if (target != null)
                    {
                        target.LastSeen = DateTime.UtcNow;
                        target.NativeValue = msg?.Request?.SceneId.ToString();
                        context.SaveChanges();
                        DeviceManager.UpdateDeviceNativeData(target.Id, new ReportItem(msg.Request.DeviceKey, new IoTItemData() { Value = msg.Request.SceneId.ToString() }, msg.IsTell));
                        return target.Id;
                    }

                    throw new ArgumentNullException("SceneEventRepresenter item not found");
                }
            }
        }

        public ZWaveKnownDeviceParameters GetKnownDeviceConfiguration(HubWorkerCommand.GetKnownDeviceConfiguration msg)
        {
            lock (Locks.GetOrAdd(msg.DeviceKey, s => new object()))
            {
                using (var context = new Entities())
                {
                    var customer = RetrieveCustomer(context, msg.UserName);
                    var thing = customer.Things.Where(x => x.State == ThingStates.Active).FirstOrDefault(f => f.NativeKey == msg.DeviceKey);
                    if (thing != null)
                    {
                        KnownDeviceRecognizer recognizer = new KnownDeviceRecognizer();
                        if (recognizer.RecognizeZWaveDevice(context, thing) && recognizer.KnownConfiguration.ConfigurationKnownItems.Count > 0)
                        {
                            Log.Verbose($"Sending known device configuration to user's hub. UserID:{customer.Id} ThingID:{thing.Id} ");
                            return recognizer.KnownConfiguration;
                        }
                    }

                    Log.Verbose($"Can't find known device configuration for DeviceKey:{msg.DeviceKey} UserID:{customer.Id}");
                    return null;
                }
            }
        }

        public void ReportDeviceReadyForCommands(ReportDeviceReadyForCommands msg)
        {
            Log.Debug($"ReportDeviceReadyForCommands {msg.DeviceKey}");
            lock (Locks.GetOrAdd(msg.DeviceKey, s => new object()))
            {
                using (var reporter = ReporterFactory.GetReporter(DeviceManager, msg.DeviceKey, msg.UserName))
                {
                    reporter.ReportDeviceReadyForCommands();
                }
            }
        }

        private Dictionary<string, string> GetCustomerSettingsAsDict(Customer customer)
        {
            return new Dictionary<string, string>
            {
                {
                    "RecordingPreMotionDetectionBufferSeconds",
                    customer.RecordingPreMotionDetectionBufferSeconds.ToString()
                },
                {
                    "RecordingPostMotionDetectionBufferSeconds",
                    customer.RecordingPostMotionDetectionBufferSeconds.ToString()
                },
                {
                    "timezone",
                    customer.RecordingPostMotionDetectionBufferSeconds.ToString()
                }
            };
        }
    }
}