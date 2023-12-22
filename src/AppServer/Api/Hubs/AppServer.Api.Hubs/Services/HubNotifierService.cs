using System;
using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Actors;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.Hub.HubApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Services
{
    public class HubNotifierService : IHubNotifierService
    {
        private static readonly object Sync = new object();
        private readonly ActorSystem _system;
        private IActorRef _actor;

        public HubNotifierService(ActorSystem system)
        {
            _system = system;
        }

        private IActorRef Provide()
        {
            lock (Sync)
            {
                if (_actor == null)
                    _actor = _system.ActorSelection($"/user/{HubConnectionManagerActor.Name}").ResolveOne(TimeSpan.FromSeconds(10)).Result;
            }

            return _actor;
        }

        public void Notify(HubDeviceManager.Notification message)
        {
            Provide().Tell(message);
        }

        public void Notify(HubDeviceManager.Notification message, IActorRef sender)
        {
            Provide().Tell(message, sender);
        }

        public void Notify(VoiceNotification message)
        {
            Provide().Tell(message);
        }

        public void SendAudio(AudioDataTransfer.ReceiveAudioData msg)
        {
            Provide().Tell(msg);
        }

        public void SendConfirmationAsk(VoiceConfirmationCommands.AskVoiceQuestion msg)
        {
            Provide().Tell(msg);
        }

        public void SendInfoVoice(AudioMessageCommands.SayAudioMessage msg)
        {
            Provide().Tell(msg);
        }

        public void DelegateHttpRequest(LocalHttpRequestDelegation msg)
        {
            Provide().Tell(msg);
        }
    }
}
