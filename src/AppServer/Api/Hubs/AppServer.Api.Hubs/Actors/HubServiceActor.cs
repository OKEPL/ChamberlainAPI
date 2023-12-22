using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.AppServer.Api.Hubs.Services;
using Chamberlain.Common.Akka;

namespace Chamberlain.AppServer.Api.Hubs.Actors
{
    public class HubServiceActor : Receiver
    {
        private readonly IHubService _hubService;

        public HubServiceActor()
        {
            _hubService = new HubService();

            Receive<HubWorkerCommand.ReportItem>(msg =>
                Context.Handle(msg, item => new HubWorkerCommand.DeviceReported(_hubService.ReportItem(item), msg.DeviceKey)));

            Receive<HubWorkerCommand.ReportThing>(msg =>
                Context.Handle(msg, item => new HubWorkerCommand.DeviceReported(_hubService.ReportThing(item), msg.DeviceKey)));

            Receive<HubWorkerCommand.ReportNodeUnpaired>(msg =>
            Context.Handle(msg, item =>
            {
                _hubService.ReportThingUnpaired(msg);
                return new HubWorkerCommand.DeviceReported(-1, null); //todo check if needs this response
            }));

            Receive<HubWorkerCommand.GetKnownDeviceConfiguration>(msg =>
                Context.Handle(msg, item => _hubService.GetKnownDeviceConfiguration(item)));

            Receive<HubWorkerCommand.ReportDeviceReadyForCommands>(msg => _hubService.ReportDeviceReadyForCommands(msg));

            Receive<HubWorkerCommand.ReportDeviceSceneEvent>(msg => _hubService.ReportDeviceSceneEvent(msg));


        }
    }       

        /*
                long ReportItem(ReportItem request);
        long ReportThing(ReportThing request);
        void ReportThingUnpaired(ReportNodeUnpaired msg);
        ZWaveKnownDeviceParameters GetKnownDeviceConfiguration(GetKnownDeviceConfiguration msg);
        void ReportDeviceReadyForCommands(ReportDeviceReadyForCommands msg);
        long ReportDeviceSceneEvent(ReportDeviceSceneEvent msg);
        (long,string) ReportItemBasicSet(ReportItemBasicSet msg);
        void ReportZWaveHardReset(ReportZWaveHardReset msg);
        CameraSettingsModel GetCameraSettings(GetCameraSettings msg);
        SettingsModel GetHubSettings(GetHubSettings msg);
        FaceRecognitionSettingsModel GetFaceRecognitionSettings(GetFaceRecognitionSettings msg);
         */

}
