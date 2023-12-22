namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera
{
    public class CameraModel : BaseChamberlainModel
    {
        public long id { get; set; }

        public long itemId { get; set; }

        public string image { get; set; }

        public bool isCurrentStreamMain { get; set; }

        public int motionDetectionLevel { get; set; }

        public string name { get; set; }

        public string previewUrl { get; set; }

        public string status { get; set; }

        public string streamUrl { get; set; }
    }
}