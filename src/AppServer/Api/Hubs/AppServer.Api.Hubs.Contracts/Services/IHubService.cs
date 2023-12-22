using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device.Camera;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Settings;
using Chamberlain.Common.Content.DataContracts.ZWave;
using static Chamberlain.AppServer.Api.Hubs.Contracts.Commands.HubWorkerCommand;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Services
{
    public interface IHubService
    {
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
    }
}
