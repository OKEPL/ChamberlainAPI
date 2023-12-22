namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera
{
    public class CameraImageModel : BaseChamberlainModel
    {
        public long id { get; set; }

        public string image { get; set; }
    }
}