using System.Collections.Generic;

using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.Common.Akka;

namespace Chamberlain.AppServer.Api.Hubs.Actors
{
    public class CameraDataTransferManager : Receiver
    {
        private Dictionary<string, IActorRef> _children = new Dictionary<string, IActorRef>();

        public CameraDataTransferManager()
        {
            Receive<CameraDataTransfer.HasDeviceKey>(msg => HandleMessage(msg.DeviceKey, msg));
        }

        protected override void PostStop()
        {
            _children.Clear();
            base.PostStop();
        }

        private void HandleMessage(string deviceKey, object message)
        {
            if (_children.ContainsKey(deviceKey))
            {
                _children[deviceKey].Forward(message);
            }
            else
            {
                var child = Context.ActorOf(Props.Create(() => new CameraDataTransferActor(deviceKey)), deviceKey);
                _children.Add(deviceKey, child);
                child.Forward(message);
            }
        }
    }
}