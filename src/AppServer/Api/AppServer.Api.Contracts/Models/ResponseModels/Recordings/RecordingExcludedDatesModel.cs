namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Recordings
{
    #region

    using System.Collections.Generic;

    #endregion

    public class RecordingExcludedDatesModel : BaseChamberlainModel
    {
        public RecordingExcludedDatesModel()
        {
            ExcludedDates = new List<string>();
        }

        public string EndDate { get; set; }

        public IEnumerable<string> ExcludedDates { get; set; }

        public string StartDate { get; set; }
    }
}