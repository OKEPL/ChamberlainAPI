using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device.Camera
{
    public class CameraSettingsModel : BaseChamberlainModel
    {
        public CameraSettingsModel()
        {
            ItemId = 0;
            BrandName = string.Empty;
            ModelName = string.Empty;
            Name = string.Empty;
            MainStreamVideoPath = string.Empty;
            SubStreamVideoPath = string.Empty;
            ImagePath = string.Empty;
            Login = string.Empty;
            Password = string.Empty;
            IsCurrentStreamMain = true;
            MotionDetectionLevel = 0;
            HostAddress = string.Empty;
            RtspPort = string.Empty;
            HttpPort = string.Empty;
        }

        public string BrandName { get; set; }

        public string HostAddress { get; set; }

        public string HttpPort { get; set; }

        [Required]
        public long ItemId { get; set; }

        public string ImagePath { get; set; }

        public bool IsCurrentStreamMain { get; set; }

        public string Login { get; set; }

        public string MainStreamVideoPath { get; set; }

        public string ModelName { get; set; }

        public int MotionDetectionLevel { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string RtspPort { get; set; }

        public string SubStreamVideoPath { get; set; }
    }
}