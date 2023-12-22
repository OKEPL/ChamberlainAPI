using System.Collections.Generic;
using Chamberlain.Common.Akka;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Hub.HubApp.Contracts.DataStructures;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands
{
    public static class HubWorkerCommand
    {
        public class GetKnownDeviceConfiguration : DeviceReport
        {

            public GetKnownDeviceConfiguration(string deviceKey) : base(deviceKey)
            {
            }
        }

        public class ReportDeviceReadyForCommands : DeviceReport
        {
            public ReportDeviceReadyForCommands(string deviceKey) : base(deviceKey)
            {
            }
        }

        public class ReportDeviceSceneEvent : Authorizable, IRouteToHubWorker
        {
            public ReportSceneEvent Request { get; set; }

            public ReportDeviceSceneEvent(ReportSceneEvent request)
            {
                Request = request;
            }
        }

        public class ReportItemBasicSet : Authorizable, IRouteToHubWorker
        {
            public ReportBasicSet Request { get; set; }

            public ReportItemBasicSet(ReportBasicSet request)
            {
                Request = request;
            }
        }

        public class ReportZWaveHardReset : DeviceReport
        {
            public ReportZWaveHardReset(string deviceKey) : base(deviceKey)
            {
            }
        }

        public class GetCameraSettings : Authorizable, IRouteToHubWorker
        {
            public long ItemId { get; }

            public GetCameraSettings(long itemId)
            {
                ItemId = itemId;
            }
        }

        public class GetFaceRecognitionSettings : Authorizable, IRouteToHubWorker, IUnstashable
        {

        }

        public class GetHubSettings : Authorizable, IRouteToHubWorker, IUnstashable
        {

        }

        public class SettingsMsg
        {
            public Dictionary<string, string> SettingsDictionary { get; }

            public SettingsMsg(Dictionary<string, string> settingsDictionary)
            {
                SettingsDictionary = settingsDictionary;
            }
        }

        public class GetSettingsCommand
        {

        }

        public class ReportNodeUnpaired : DeviceReport
        {
            public ReportNodeUnpaired(string deviceKey) : base(deviceKey)
            {
            }
        }

        public class ReportThing : DeviceReport
        {
            public IoTThingData DeviceData { get; set; }

            public ReportThing(string deviceKey, IoTThingData data, bool isTell = true) : base(deviceKey, isTell)
            {
                DeviceData = data;
            }
        }

        public class ReportItem : DeviceReport
        {
            public IoTItemData DeviceData { get; set; }

            public ReportItem(string deviceKey, IoTItemData data, bool isTell = true) : base(deviceKey, isTell)
            {
                DeviceData = data;
            }
        }

        public class DeviceReport : Authorizable, IRouteToHubWorker
        {
            public string DeviceKey { get; set; }

            public DeviceReport(string deviceKey, bool isTell = true) : base(isTell)
            {
                DeviceKey = deviceKey;
            }
        }

        public class DeviceReported
        {
            public long Id { get; }
            public string Key { get; }

            public DeviceReported()
            {

            }

            public DeviceReported(long id, string key)
            {
                Id = id;
                Key = key;
            }
        }
    }
}
