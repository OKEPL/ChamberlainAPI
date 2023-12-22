using Chamberlain.AppServer.Devices.Interfaces;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Database.Persistency.Model;

namespace Chamberlain.AppServer.Api.Hubs.Reporters
{
    public static class ReporterFactory
    {
        
        public static IReporter GetReporter(IDeviceManager deviceManager, string deviceKey, string userName)
        {
            var keys = deviceKey.Split('_');
            switch (keys[0])
            {
                case ItemTypes.HubCamera:
                case ItemTypes.VideoIntercomCamera:
                case ItemTypes.KinectDepthCamera:
                    return new CameraReporter(deviceManager, deviceKey, userName);
                case ItemTypes.Zwave:
                    return new ZWaveReporter(deviceManager, deviceKey, userName);
                case ItemTypes.Zmote:
                    return new ZmoteReporter(deviceManager, deviceKey, userName);
                default:
                    return new DefaultReporter(deviceManager, deviceKey, userName);
            }
        }

        public static IReporter GetReporter(IDeviceManager deviceManager, string deviceKey, string userName, Entities context)
        {
            var keys = deviceKey.Split('_');
            switch (keys[0])
            {
                case ItemTypes.HubCamera:
                case ItemTypes.VideoIntercomCamera:
                case ItemTypes.KinectDepthCamera:
                    return new CameraReporter(deviceManager, deviceKey, userName, context);
                case ItemTypes.Zwave:
                    return new ZWaveReporter(deviceManager, deviceKey, userName, context);
                case ItemTypes.Zmote:
                    return new ZmoteReporter(deviceManager, deviceKey, userName, context);
                default:
                    return new DefaultReporter(deviceManager, deviceKey, userName, context);
            }
        }
    }
}