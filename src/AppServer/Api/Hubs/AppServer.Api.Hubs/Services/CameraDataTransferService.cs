using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.AppServer.Devices.Devices.Camera;
using Chamberlain.AppServer.Devices.Interfaces;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Hubs.Services
{
    public class CameraDataTransferService : ICameraDataTransferService
    {
        [SetterProperty] public IDeviceManager DeviceManager { get; set; }

        public void HandleTsChunk(CameraDataTransfer.SendTsChunk command)
        {
            var device = DeviceManager.GetDeviceByDeviceKey(command.DeviceKey);
            if (device is HubCameraActor hubCamera)
            {
                hubCamera.GetActorRef().Tell(command);
            }
        }
    }
}
