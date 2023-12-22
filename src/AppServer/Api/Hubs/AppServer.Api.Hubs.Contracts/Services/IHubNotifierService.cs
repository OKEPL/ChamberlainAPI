using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications;
using Chamberlain.Hub.HubApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Services
{
    public interface IHubNotifierService
    {
        void Notify(HubDeviceManager.Notification message);
        void Notify(VoiceNotification message);
        void Notify(HubDeviceManager.Notification message, IActorRef sender);
        void SendAudio(AudioDataTransfer.ReceiveAudioData msg);
        void SendConfirmationAsk(VoiceConfirmationCommands.AskVoiceQuestion msg);
        void SendInfoVoice(AudioMessageCommands.SayAudioMessage msg);
        void DelegateHttpRequest(LocalHttpRequestDelegation msg);
    }
}