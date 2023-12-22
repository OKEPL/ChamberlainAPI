namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera
{
    #region

    using System.Collections.Generic;

    #endregion

    public class SupportedCameraBrandModel : BaseChamberlainModel
    {
        public SupportedCameraBrandModel(string brandName)
        {
            this.Brand = brandName;
        }

        public string Brand { get; set; }

        public List<SupportedCameraModel> CameraModels { get; set; } = new List<SupportedCameraModel>();
    }
}