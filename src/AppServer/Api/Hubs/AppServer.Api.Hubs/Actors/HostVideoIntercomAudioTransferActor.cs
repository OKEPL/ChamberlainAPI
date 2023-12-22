using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Helpers;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;
using System.Threading.Tasks;

namespace Chamberlain.AppServer.Api.Hubs.Actors
{
    internal class HostVideoIntercomAudioTransferActor : Receiver
    {
        [SetterProperty]
        public IVideoIntercomAudioTransferService AudioTransferService { get; set; }

        public HostVideoIntercomAudioTransferActor()
        {
            Receive<AudioDataTransfer.SendAudioData>(msg => SendAudioData(msg));
        }

        private void SendAudioData(ILargePacketMessage msg)
        {
            Task.Run(() => LargePacketMessageSpooler.Instance.HandleLargePacketMessage(
                msg, obj => AudioTransferService.ReceiveAudioData((AudioDataTransfer.SendAudioData)obj)));
            
            Context.Sender.Tell(new Response());
        }
    }
}
