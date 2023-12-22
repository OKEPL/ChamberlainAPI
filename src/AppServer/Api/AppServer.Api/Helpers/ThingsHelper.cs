using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Content.DataContracts.ZWave;
using Chamberlain.Common.Settings;
using Chamberlain.Database.Persistency.Model;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device;
using Chamberlain.Common.Contracts.Enums;
using Chamberlain.Database.Persistency.Model.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Chamberlain.AppServer.Api.Helpers
{
    public static class ThingsHelper
    {
        private const string DefaultIconPath = @"/Content/images/device_icon.png";

        public static Thing GetControllerByThingIdOrFindDefaultController(Entities context, string userName, long? thingId)
        {
            var things = context.Customers.First(x => x.Username.Equals(userName)).Things;

            Thing controller = null;

            if (thingId.HasValue)
                controller = things.First(x => x.Id == thingId);

            if (controller == null)
                controller = things.First(x => BaseDeviceSettings.GetThingSettings(x) is IIoTThingSettings iotThingSettings && iotThingSettings.IsController);

            return controller;
        }

        public static DeviceModel MapThingToDeviceModel(Thing thing)
        {
            var deviceModel = new DeviceModel();
            if (thing != null)
            {
                var thingKeyModel = thing.GetNativeKeyModel();
                deviceModel = new DeviceModel
                {
                    thing_id = thing.Id,
                    node_id = int.TryParse(thingKeyModel?.NodeId, out int nodeId) ? nodeId : (int?)null,
                    thing_name = thing.GivenName,
                    native_name = thing.NativeName,
                    icon = thing.KnownDevice?.IconPath ?? DefaultIconPath,
                    home_id = long.TryParse(thingKeyModel?.HomeId, out long homeId) ? homeId : (long?)null,
                    scenes = thing.ScenesThingsBindings.AsEnumerable().Select(s => s.Scene.ToSceneModel()).ToList(),
                    device_type = byte.TryParse(thingKeyModel?.DeviceType, out byte devType) ? devType : (byte?)null,
                    device_db_type = thing.Items.FirstOrDefault()?.Type,
                    is_controller = !thing.Items.Any(x => x.KnownItem?.IsAction == true || x.KnownItem?.IsTrigger == true)
                };

                if (thing.KnownDevice == null)
                {
                    deviceModel.native_name = "Device is pairing...";
                    return deviceModel;
                }

                var itemPosition = 0;

                foreach (var item in thing.Items.Where(x => x.KnownItem != null))
                {
                    itemPosition++;
                    try
                    {
                        var model = MapItemToItemModel(item, itemPosition);
                        deviceModel.items.Add(model);
                        deviceModel.status = DetermineDeviceStatus(model.last_seen);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Error mapping item to GUI item model. ID:{item.Id}, native key:{item.NativeKey}, native name:{item.NativeName}");
                        throw;
                    }
                }
            }

            deviceModel.items = deviceModel.items.OrderBy(i => i.position).ToList();
            return deviceModel;
        }

        private static NewItemModel MapItemToItemModel(Item item, int position)
        {
            var knownSettings = BaseDeviceSettings.GetKnownItemSettings(item.KnownItem);
            if (knownSettings == null || item.KnownItem.IsHidden) return new NewItemModel();

            var model = new NewItemModel
            {
                item_id = item.Id,
                type = item.Type,
                native_name = item.NativeName,
                custom_name = item.CustomName,
                category = (int)DeviceItemCategory.Sensor,
                last_seen = item.LastSeen ?? DateTime.UtcNow,
                icon = CachedSettings.Get("DefaultZWaveItemIconPath", "/Content/images/device_multi_sensor.png"),
                UXControlType = (int)knownSettings.WebControlType,
                read_only = item.KnownItem.IsAction == false,
                value = string.IsNullOrEmpty(item.NativeKey) ? "(Not reported)" : item.NativeValue,
                position = position,
                list_value_type_items = knownSettings.ValueWithLabelOption?.PossibleValues.Select(x => x.Value).ToList()
            };

            switch (knownSettings.WebControlType)
            {
                case WebControlType.Numeric:
                    model.units = knownSettings.NumericValueOption?.Unit;
                    model.ValueRange = new IoTItemValueRange
                    {
                        MaxValue = (decimal)knownSettings.NumericValueOption.MaxValue,
                        MinValue = (decimal)knownSettings.NumericValueOption.MinValue,
                        Step = (decimal)knownSettings.NumericValueOption.Step
                    };
                    break;
                case WebControlType.ComboBox:
                    var index = -1;
                    if (knownSettings.ValueWithLabelOption != null)
                        index = knownSettings.ValueWithLabelOption.PossibleValues.FindIndex(valWithLabel =>
                            valWithLabel.Value == model.value);

                    if (index >= 0 && knownSettings.ValueWithLabelOption != null)
                        model.value = Regex.Replace(knownSettings.ValueWithLabelOption.PossibleValues[index].Label, @"\bbutton\b", "", RegexOptions.IgnoreCase);
                    break;
            }

            return model;
        }

        private static string DetermineDeviceStatus(DateTime lastSeen)
        {
            var now = DateTime.UtcNow;
            if ((now - lastSeen).TotalMinutes > 30)
                return DeviceModel.DeviceStatus.Dead.ToString("G");

            if ((now - lastSeen).TotalMinutes > 15)
                return DeviceModel.DeviceStatus.Warning.ToString("G");

            return DeviceModel.DeviceStatus.OK.ToString("G");
        }
    }
}