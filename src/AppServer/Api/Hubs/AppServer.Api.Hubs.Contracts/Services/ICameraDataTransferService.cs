using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Services
{
    public interface ICameraDataTransferService
    {
        void HandleTsChunk(CameraDataTransfer.SendTsChunk command);
    }
}