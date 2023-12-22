using System;
using System.IO;
using System.Linq;

using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Devices.Devices.Camera;
using Chamberlain.Common.Akka;
using Chamberlain.Common.Settings;
using Chamberlain.Database.Persistency.Model;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Hubs.Actors
{
    public class CameraDataTransferActor : Receiver
    {
        private readonly long _customerId;
        private IActorRef _hubCameraRef;

        public CameraDataTransferActor(string deviceKey)
        {
            using (var context = new Entities())
            {
                var item = context.Items
                    .Include(i => i.Thing)
                    .FirstOrDefault(x => x.NativeKey == deviceKey);
                _customerId = item.Thing.CustomerId;
                _hubCameraRef = Context.ActorOf(Props.CreateBy<HubActorProducer>(item.Id, item.Thing.CustomerId, item.CustomName, item.Type), $"{deviceKey}_hubCamera");
            }

            Receive<CameraDataTransfer.GetChunkSettings>(msg => HandleCameraSettings());
            Receive<CameraDataTransfer.UpdateCamera> (msg => _hubCameraRef.Forward(msg));
        }

        private void HandleCameraSettings()
        {
            var chunksMainFolder = CachedSettings.Get<string>($"FfmpegChunksPath.{Environment.OSVersion.Platform}").Replace("{userId}", _customerId.ToString());
            Context.Sender.Tell(new CameraDataTransfer.GetChunkSettingsResponse(chunksMainFolder,
                Path.Combine(chunksMainFolder, CachedSettings.Get("PreviewFolderName", "preview")),
                Path.Combine(chunksMainFolder, CachedSettings.Get("RecordingFolderName", "recording")),
                CachedSettings.Get<int>("HubCameraListSize", 3),
                CachedSettings.Get<int>("HubCameraChunkLengthSeconds", 2)));
        }

        private class HubActorProducer : IIndirectActorProducer
        {
            private readonly long _itemId;
            private readonly long _customerId;
            private readonly string _type;
            private readonly string _name;

            public HubActorProducer()
            {
            }

            public HubActorProducer(params object[] args)
            {
                _itemId = (long)args[0];
                _customerId = (long)args[1];
                _name = (string)args[2];
                _type = (string)args[3];
            }

            public Type ActorType => typeof(HubCameraActor);

            public ActorBase Produce()
            {
                var child = new HubCameraActor(_itemId, _customerId, _name, _type, null, ActorRefs.Nobody);
                child.Start();
                return child;
            }

            public void Release(ActorBase actor)
            {

            }
        }
    }
}