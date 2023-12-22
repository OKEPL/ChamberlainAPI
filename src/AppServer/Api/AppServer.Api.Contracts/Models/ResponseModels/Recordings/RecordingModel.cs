namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Recordings
{
    #region

    using System;

    #endregion

    public class RecordingModel : BaseChamberlainModel
    {
        public DateTime dateTime { get; set; }

        public string description { get; set; }

        public long duration { get; set; }

        public long id { get; set; }

        public string itemName { get; set; }

        public string recording_status { get; set; }

        public string recordingUrl { get; set; }

        public long size { get; set; }

        public string thumbnailUrl { get; set; }

        public bool watched { get; set; }
    }
}