using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Services
{
    public interface IVideoIntercomAudioTransferService
    {
        void ReceiveAudioData(AudioDataTransfer.SendAudioData msg);
    }
}