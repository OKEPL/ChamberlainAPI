namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using System.Collections.Generic;

    using Chamberlain.AppServer.Api.Contracts.DataTransfer;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device.Camera;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera;

    #endregion

    public interface ICameraService
    {
        void DeleteCamera(string userName, long thingId);

        CameraModel GetCameraByItem(string userName, long itemId);

        CameraSettingsModel GetCameraByItemExt(string userName, long itemId);

        CameraModel GetCameraByThing(string userName, long thingId);

        List<CameraSettingsModel> GetCamerasExt(string userName);

        List<SupportedCameraBrandModel> GetSupportedCameras();

        CheckResultModel TestHostPort(int port, string ip);

        CheckResultModel TestRtspPort(int port, string ip);

        void UpdateCamera(string userName, CameraSettingsModel cameraSettingsModel);
    }
}