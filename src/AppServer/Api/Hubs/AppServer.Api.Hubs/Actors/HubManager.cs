using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Helpers;
using Chamberlain.Common.Akka;
using Chamberlain.Common.Content.Commands;
using Chamberlain.Common.Settings;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.Hub.HubApp.Contracts.Commands;
using Chamberlain.Plugins.ObjectTracker.Contracts.Commands;
using Chamberlain.Plugins.Overseer.Communication;
using Chamberlain.Plugins.Overseer.Contracts;
using Common.RemoteLogger.Messages;
using Plugins.RemindersSender.Contracts.Commands;
using Plugins.UserLocation.Communication;
using Plugins.UserLocation.Contracts;
using Plugins.VoiceCommandExecutor.Communication;
using Serilog;

namespace Chamberlain.AppServer.Api.Hubs.Actors
{
    public class HubManager : Receiver
    {
        private readonly IActorRef _voiceActor;
        private readonly string _userName;
        private readonly long _customerId;
        private IActorRef _cameraDataTransferActor;
        private IActorRef _videoIntercomAudioTransferActor;
        private IActorRef _workerActor;
        private IActorRef _overseerActor;

        public HubManager(IActorRef voiceActorRef, string userName)
        {
            _voiceActor = voiceActorRef;
            _userName = userName;

            Receive<HubConnectedMessage>(message => InitializeHubConnection(message.HubActorRef));
            Receive<Authorizable>(message => CheckAuthorization(message));
            Receive<AudioDataTransfer.SendAudioData>(message => _videoIntercomAudioTransferActor.Forward(message));
            Receive<TriggerMessage>(message => Context.Parent.Forward(message));
            Receive<GetUserTracking>(message => Forward(message));
            Receive<InitReminder>(msg => Forward(msg));

            Receive<IRouteToUserLocation>(message => HandleUserLocation(message));
            Receive<IRouteToCameraDataTransfer>(message => _cameraDataTransferActor.Forward(message));
            Receive<IRouteToVoiceInterpretation>(message => HandleVoiceInterpretation(message, _voiceActor));
            Receive<IRouteToCameraDataTransfer>(message => Forward(_cameraDataTransferActor, message));
            Receive<IRouteToHubWorker>(message => _workerActor.Forward(message));
            Receive<IRouteFromOverseer>(message => Forward(message));
            Receive<IRouteToOverseer>(message => _overseerActor.Forward(message));
            Receive<IErrorNotification>(message => message.Log());

            using (var context = new Entities())
                _customerId = context.Customers.First(customer => customer.Username == userName).Id;
        }

        protected override void PreStart()
        {
            InitHubWorker();
            InitDataTransferActor();
            InitOverseer();
            InitVideoIntercom();
            
            base.PreStart();
        }

        private static void HandleVoiceInterpretation(IRouteToVoiceInterpretation message, IActorRef voiceActorRef)
        {
            var voiceCommandExecutor = Context.ActorOf(Props.Create(() => new VoiceCommandExecutorActor(voiceActorRef)));
            voiceCommandExecutor.Forward(message);
        }

        private static void HandleUserLocation(IRouteToUserLocation message)
        {
            var userLocationActor = Context.ActorOf(Props.Create(() => new UserLocationActor(Context.Self)));
            userLocationActor.Forward(message);
        }

        private void InitializeHubConnection(IActorRef hubActorRef)
        {
            hubActorRef.Tell(new HubGatewayCommand.Connected(Self));
        }

        private void CheckAuthorization(Authorizable message)
        {
            if (string.IsNullOrEmpty(message.Token))
            {
                Log.Error("Received message with empty token.");
                return;
            }

            if (TokenValidationHelper.Validate(message.Token, out var userName))
            {
                message.UserName = userName;

                switch (message)
                {
                    case AudioDataTransfer.SendAudioData audio:
                        _videoIntercomAudioTransferActor.Forward(audio);
                        break;
                    case IRouteToVoiceInterpretation voice:
                        HandleVoiceInterpretation(voice, _voiceActor);
                        break;
                    case IRouteToCameraDataTransfer camera:
                        Forward(_cameraDataTransferActor, camera);
                        break;
                    case IRouteToHubWorker worker:
                        _workerActor.Forward(worker);
                        break;
                    case IRouteToOverseer overseer:
                        _overseerActor.Forward(overseer);
                        break;
                }
            }
            else
            {
                Log.Error("Received message with invalid JWT token.");
            }
        }

        private void InitVideoIntercom()
        {
            _videoIntercomAudioTransferActor = 
                Context.ActorOf(Context.DI().Props<HostVideoIntercomAudioTransferActor>()
                       .WithRouter(new ConsistentHashingPool(5).WithHashMapping(msg => nameof(msg))), "audio-data-transfer");
        }

        private void InitOverseer()
        {
            if (!CachedSettings.Get("EnableFaceRecognitionLearning", false))
                return;

            _overseerActor = Context.ActorOf(Props.Create(() => new OverseerActor(_userName, _customerId)), "overseer");
        }

        private void InitHubWorker()
        {
            _workerActor = 
                Context.ActorOf(Context.DI().Props<HubWorkerActor>()
                       .WithRouter(new ConsistentHashingPool(12).WithHashMapping(msg => nameof(msg))), "worker");
        }

        private void InitDataTransferActor()
        {
            _cameraDataTransferActor = Context.ActorOf(Context.DI().Props<CameraDataTransferManager>(), "camera-data-transfer");
        }
    }
}
