using Chamberlain.AppServer.Devices.Interfaces;
using Chamberlain.Common.Content.DataContracts.Camera;
using Chamberlain.Common.Domotica;
using Chamberlain.Database.Persistency.Model;
using System;
using Chamberlain.Common.Contracts.Enums;

namespace Chamberlain.AppServer.Api.Hubs.Reporters
{
    public class CameraReporter : DefaultReporter
    {
        public CameraReporter(IDeviceManager deviceManager, string deviceKey, string userName)
            : base(deviceManager, deviceKey, userName)
        {
        }

        public CameraReporter(IDeviceManager deviceManager, string deviceKey, string userName, Entities context)
            : base(deviceManager, deviceKey, userName, context)
        {
        }

        protected override void PostAddThing()
        {
            GetThing().Items.Add(SettingsItem);
            GetThing().Items.Add(RecordingItem);

            ReportDeviceReadyForCommands();

            base.PostAddThing();
        }

        protected override void PostAddItem()
        {
            RecognizeItemIfNeeded();

            base.PostAddItem();
        }
        /// <summary>
        /// Representing camera item with simple native_key to manage camera in DeviceManager (ex. processing chunks). Item must be unique.
        /// </summary>
        private Item SettingsItem =>
            new Item
            {
                NativeKey = DeviceKey,
                NativeValue = null,
                CustomName = MockSpecialItemType.Settings.ToString(),
                NativeName = MockSpecialItemType.Settings.ToString(),
                Type = ThingNativeKeyModel.FromNativeKey(DeviceKey).DatabaseDeviceType,
                ThingId = GetThing().Id,
                Settings = XmlSerialization.ToString(CameraSettings),
                LastSeen = DateTime.UtcNow,
            };

        private Item RecordingItem =>
            new Item
            {
                NativeKey = ItemNativeKeyModel.FromThingKeyModel(ThingNativeKeyModel.FromNativeKey(DeviceKey), MockSpecialItemType.Recording.ToString(), null).ToString(),
                NativeValue = null,
                CustomName = MockSpecialItemType.Recording.ToString(),
                NativeName = MockSpecialItemType.Recording.ToString(),
                Type = ThingNativeKeyModel.FromNativeKey(DeviceKey).DatabaseDeviceType,
                ThingId = GetThing().Id,
                Settings = null,
                LastSeen = DateTime.UtcNow,
            };

        private CameraSettings CameraSettings => new CameraSettings
        {
            IsCurrentStreamMain = true,
            Login = "solomiocam",
            Password = "solomiopass1",
            MotionDetectionLevel = 5,
            OutputStreamName = DeviceKey
        };
    }
}