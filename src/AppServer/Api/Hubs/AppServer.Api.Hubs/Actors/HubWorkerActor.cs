using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.Common.Akka;
using Chamberlain.Common.Content.DataContracts.ZWave;
using StructureMap.Attributes;
using Plugins.VoiceCommandPlugin;
using VoiceApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Actors
{
    public class HubWorkerActor : Receiver
    {
        [SetterProperty]
        public IHubService HubService { get; set; }
        [SetterProperty]
        public IVoiceUtilsService VoiceUtilsService { get; set; }     
        [SetterProperty]
        public IVoiceCommandPlugin VoiceCommandPlugin { get; set; }

        public HubWorkerActor()
        {
            Receive<HubWorkerCommand.ReportThing>(msg => ReportNode(msg));
            Receive<HubWorkerCommand.ReportNodeUnpaired>(msg => ReportNodeUnpaired(msg));
            Receive<HubWorkerCommand.ReportItem>(msg => ReportGenericDevice(msg));
            Receive<HubWorkerCommand.GetKnownDeviceConfiguration>(msg => GetKnownDeviceConfiguration(msg));
            Receive<HubWorkerCommand.ReportDeviceReadyForCommands>(msg => ReportDeviceReadyForCommands(msg));
            Receive<HubWorkerCommand.ReportDeviceSceneEvent>(msg => ReportDeviceSceneEvent(msg));
            Receive<HubWorkerCommand.ReportItemBasicSet>(msg => ReportItemBasicSet(msg));
            Receive<HubWorkerCommand.ReportZWaveHardReset>(msg => ReportZWaveHardReset(msg));
            Receive<HubWorkerCommand.GetCameraSettings>(msg => GetCameraSettings(msg));
            Receive<HubWorkerCommand.GetHubSettings>(msg => GetHubSettings(msg));
            Receive<HubWorkerCommand.GetFaceRecognitionSettings>(msg => GetFaceRecognitionSettings(msg));

            Receive<SynthesizerCommand.DownloadRequest>(msg => ReceiveSoundDownloadRequest (msg));
            Receive<SynthesizerCommand.GetDefaultSoundsList>(msg => GetDefaultSoundsList(msg));

            Receive<RecognitionCommand.GoogleSpeechPartialRequest.GoogleSpeechPartialRequestPart>(msg => ReceivePartialRequest(msg));

            Receive<GrammarUpdateCommand.AskForUpdate>(msg => SendGrammarUpdate(msg));
        }
        
        private void ReceiveSoundDownloadRequest(SynthesizerCommand.DownloadRequest msg)
        {
            VoiceUtilsService.ReceiveSoundDownloadRequest(msg.Text, Sender, msg.VoiceUnitId);
        }

        private void GetHubSettings(HubWorkerCommand.GetHubSettings msg)
        {
            Context.Handle(msg, item => HubService.GetHubSettings(item));
        }

        private void GetCameraSettings(HubWorkerCommand.GetCameraSettings msg)
        {
            Context.Handle(msg, item => HubService.GetCameraSettings(item));
        }

        private void GetFaceRecognitionSettings(HubWorkerCommand.GetFaceRecognitionSettings msg)
        {
            Context.Handle(msg, item => HubService.GetFaceRecognitionSettings(msg));
        }

        private void ReportZWaveHardReset(HubWorkerCommand.ReportZWaveHardReset msg)
        {
            HubService.ReportZWaveHardReset(msg);
        }

        private void ReportItemBasicSet(HubWorkerCommand.ReportItemBasicSet msg)
        {
            HubService.ReportItemBasicSet(msg);
        }

        private void ReportDeviceSceneEvent(HubWorkerCommand.ReportDeviceSceneEvent msg)
        {
            HubService.ReportDeviceSceneEvent(msg);
        }

        private void ReportDeviceReadyForCommands(HubWorkerCommand.ReportDeviceReadyForCommands msg)
        {
            HubService.ReportDeviceReadyForCommands(msg);
        }

        private void GetKnownDeviceConfiguration(HubWorkerCommand.GetKnownDeviceConfiguration msg)
        {
            var configuration = HubService.GetKnownDeviceConfiguration(msg);

            Sender.Tell(new Response<ZWaveKnownDeviceParameters>(configuration));
        }

        private void ReportGenericDevice(HubWorkerCommand.ReportItem msg)
        {
            Context.Handle(msg, item =>
            {
                var id = HubService.ReportItem(item);
                return new HubWorkerCommand.DeviceReported(id, msg.DeviceKey);
            });
        }

        private void ReportNode(HubWorkerCommand.ReportThing msg)
        {
            Context.Handle(msg, item =>
            {
                var id = HubService.ReportThing(msg);
                return new HubWorkerCommand.DeviceReported(id, msg.DeviceKey);
            });
        }

        private void ReportNodeUnpaired(HubWorkerCommand.ReportNodeUnpaired msg)
        {
            Context.Handle(msg, item =>
            {
                HubService.ReportThingUnpaired(msg);
                return new HubWorkerCommand.DeviceReported(-1, null); //todo check if needs this response
            });
        }

        private void ReceivePartialRequest(RecognitionCommand.GoogleSpeechPartialRequest.GoogleSpeechPartialRequestPart msg)
        {
            VoiceUtilsService.ReceivePartialRequestPart(Sender, msg.RequestHash, msg.SlicesCount, msg.SliceId, msg.Data, msg.VoiceUnitId, msg.RequestId, msg.Context);
        }

        private void GetDefaultSoundsList(SynthesizerCommand.GetDefaultSoundsList msg)
        {
            VoiceUtilsService.GetDefaultSoundsList(msg.Language, Sender, msg.RequestId, msg.VoiceUnitId);
        }

        private void SendGrammarUpdate(VoiceToCloudRequest msg)
        {
            VoiceUtilsService.SendGrammarUpdate(Sender, msg.UserName, msg.VoiceUnitId);
        }
    }
}
