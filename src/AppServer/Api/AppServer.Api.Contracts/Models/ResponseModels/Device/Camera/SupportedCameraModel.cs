using Chamberlain.Common.Contracts.Enums;
using System.Collections.Generic;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera
{
    public class SupportedCameraModel : BaseChamberlainModel
    {
        public List<CameraParameters> Parameters = new List<CameraParameters>();

        public string Model { get; set; }
    }
}