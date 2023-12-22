using System;
using System.Collections.Generic;
using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications;
using Chamberlain.AppServer.Api.Hubs.Helpers;
using Chamberlain.Common.Akka;
using Chamberlain.Common.Content.Commands;
using Chamberlain.Hub.HubApp.Contracts.Commands;
using Chamberlain.Plugins.ObjectTracker.Contracts.Commands;
using Chamberlain.Plugins.Overseer.Contracts;
using Chamberlain.Services.CustomerService.Api.Contracts.Comands;
using Common.RemoteLogger.Messages;
using HubApp.Contracts.Services;
using Plugins.RemindersSender.Communication;
using Plugins.RemindersSender.Contracts.Commands;
using Plugins.UserLocation.Contracts.Commands;
using Serilog;

namespace Chamberlain.AppServer.Api.Hubs.Actors
{
    public class HubConnectionManagerActor : Receiver
    {
        public const string Name = "connections";

        private readonly Dictionary<string, HubConnectionDescription> _currentConnections = new Dictionary<string, HubConnectionDescription>();
        private readonly IActorRef _voiceActorRef;
        private IActorRef _reminderSenderActor;

        public HubConnectionManagerActor(IActorRef rulesEngineActorRef, IActorRef voiceActorRef)
        {
            _voiceActorRef = voiceActorRef;

            Receive<HubGatewayCommand.Connect>(connected => HandleHubConnection(connected));
            Receive<HubDisconnected>(message => HandleHubDisconnection(message));
            Receive<HubDeviceManager.Notification>(message => HandleNotification(message));
            Receive<AudioDataTransfer.ReceiveAudioData>(msg => ReceiveAudioData(msg));
            Receive<GetLocationForReminder>(message => GetLocationForReminder(message));
            Receive<Authorizable>(message => CheckAuthorization(message));
            Receive<TriggerActivatedMessage>(message => rulesEngineActorRef.Forward(message));
            Receive<TriggerDeactivatedMessage>(message => rulesEngineActorRef.Forward(message));
            Receive<VoiceConfirmationCommands.ReceiveVoiceAnswer>(message => rulesEngineActorRef.Forward(message));
            Receive<PingHub>(message => PingHub(message));
            Receive<InitReminder>(message => _reminderSenderActor.Forward(message));

            Receive<RouteToHubGateway>(message => ForwardToHub(message));
        }

        protected override void PostRestart(Exception reason)
        {
            base.PostRestart(reason);
            Log.Information($"Current connected hubs count: {_currentConnections.Count}");
        }

        protected override void PreStart()
        {
            _reminderSenderActor = Context.ActorOf(Props.Create(() => new ReminderSenderActor()), ReminderSenderActor.Name);
            base.PreStart();
        }

        private void HandleHubDisconnection(HubDisconnected message)
        {
            Log.Warning($"Network connection with hub for user: { message.UserName } failed.");

            var connectionDescription = _currentConnections.ContainsKey(message.UserName) ? _currentConnections[message.UserName] : null;
            if (connectionDescription != null)
            {
                Log.Warning($"Hub connection to user {message.UserName} is terminated. Stopping manager for user");
                Context.Unwatch(connectionDescription.HubActorRef);
                Context.Stop(connectionDescription.ManagerActorRef);
                _currentConnections.Remove(message.UserName);
            }
        }

        private void HandleNotification(HubDeviceManager.Notification message)
        {
            if (GetConnectedHub(message.UserName) is IActorRef act)
            {
                Log.Debug($"Sent notification for user={message.UserName} device={message.DeviceKey} action='{message.DeviceAction}' details='{message.ActionDetails}'.");
                act.Forward(message);
            }
        }

        private void ReceiveAudioData(AudioDataTransfer.ReceiveAudioData msg)
        {
            if (GetConnectedHub(msg.UserName) is IActorRef act)
            {
                act.Tell(msg);
            }
        }

        private void GetLocationForReminder(GetLocationForReminder message)
        {
            if (GetConnectedManager(message.UserName) is IActorRef act)
            {
                act.Forward(message);
            }
            else
            {
                Log.Warning($"HubManager for {message.UserName} was not found.");
            }
        }

        private void CheckAuthorization(Authorizable message)
        {
            if (string.IsNullOrEmpty(message.Token))
            {
                Log.Warning("Received message with empty token.");
                return;
            }

            if (TokenValidationHelper.Validate(message.Token, out var userName))
            {
                message.UserName = userName;

                if (GetConnectedManager(message.UserName) is IActorRef act)
                {
                    Forward(act, message);
                }
                else
                {
                    Log.Warning($"HubManager for {message.UserName} was not found.");
                }
            }
            else
            {
                Log.Warning("Received message with invalid JWT token.");
            }
        }

        private void PingHub(PingHub message)
        {
            var isConnected = GetConnectedHub(message.UserName) != null;
            Sender.Tell(isConnected);
        }

        private void ForwardToHub(RouteToHubGateway message)
        {
            if (GetConnectedHub(message.UserName) is IActorRef actorRef)
            {
                actorRef.Forward(message);
            }
        }

        private IActorRef GetConnectedHub(string userName)
        {
            return GetConnectionDescription(userName)?.HubActorRef;
        }

        private IActorRef GetConnectedManager(string userName)
        {
            return GetConnectionDescription(userName)?.ManagerActorRef;
        }

        private HubConnectionDescription GetConnectionDescription(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName) && _currentConnections.ContainsKey(userName))
            {
                return _currentConnections[userName];
            }

            return null;
        }

        private void StopUserHub(string userName)
        {
            var crntUserConnection = _currentConnections.ContainsKey(userName) ? _currentConnections[userName] : null;
            if (crntUserConnection != null && !crntUserConnection.ManagerActorRef.IsNobody())
            {
                Log.Information($"Stopping local hub manager actor for user {userName}");
                Context.Stop(crntUserConnection.ManagerActorRef);
            }

            if (crntUserConnection != null)
            {
                _currentConnections.Remove(userName);
            }
        }

        private void HandleHubConnection(HubGatewayCommand.Connect message)
        {
            if (string.IsNullOrEmpty(message.Token))
            {
                Log.Warning("Received message with empty token.");
                return;
            }

            if (TokenValidationHelper.Validate(message.Token, out var userName))
            {
                Log.Information($"Incoming new hub connection from user: {userName}. Connected hubs:{_currentConnections.Count}");

                var crntConnection = _currentConnections.ContainsKey(userName) ? _currentConnections[userName] : null;

                if (crntConnection != null && !crntConnection.ManagerActorRef.IsNobody())
                {
                    Log.Information($"Hub already connected: {userName}");
                    Context.Unwatch(crntConnection.HubActorRef);
                    crntConnection.HubActorRef = Sender;
                    crntConnection.ManagerActorRef.Tell(new HubConnectedMessage(Sender));
                    Context.WatchWith(Sender, new HubDisconnected(userName));
                    return;
                }

                StopUserHub(userName);

                var userHubManager = Context.ActorOf(Props.Create(() => 
                    new HubManager(_voiceActorRef, userName)), $"{typeof(HubManager).Name}_user_{userName}");

                _currentConnections.Add(userName, new HubConnectionDescription
                {
                    HubActorRef = Context.Sender,
                    ManagerActorRef = userHubManager
                });

                Context.WatchWith(Sender, new HubDisconnected(userName));
                userHubManager.Tell(new HubConnectedMessage(Sender));
            }
        }

        private class HubConnectionDescription
        {
            public IActorRef HubActorRef { get; set; }
            public IActorRef ManagerActorRef { get; set; }
        }

        public class HubDisconnected
        {
            public HubDisconnected(string userName)
            {
                UserName = userName;
            }

            public string UserName { get; }
        }
    }
}
