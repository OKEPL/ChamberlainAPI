using System;
using System.Collections.Generic;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera
{
    public class FaceRecognitionSettingsModel : BaseChamberlainModel
    {
        public FaceRecognitionSettingsModel()
        {
            RecognizerName = string.Empty;
            ActiveRecognizerUri = null;
            LabelsByProfiles = null;
        }

        public string RecognizerName { get; set; }
        public Uri ActiveRecognizerUri { get; set; }
        public List<(int Label, long Profile)> LabelsByProfiles { get; set; }
        public int ModelWidth { get; set; }
        public int BatchSize { get; set; }
        public int InputWidth { get; set; }
        public int InputHeight { get; set; }
        public int InputChannels { get; set; }
        public string InputName { get; set; }
        public string OutputName { get; set; }
    }
}
