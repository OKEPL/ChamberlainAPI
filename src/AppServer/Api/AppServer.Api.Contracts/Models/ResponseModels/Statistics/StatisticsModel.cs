namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Statistics
{
    #region

    using System.Collections.Generic;

    #endregion

    public class StatisticsModel : BaseChamberlainModel
    {
        public StatisticsModel()
        {
            this.NotificationCountLastWeek = new List<long>();
        }

        public CurrentModeModel CurrentModeModel { get; set; }

        public FreeSpaceModel FreeSpaceModel { get; set; }

        public IEnumerable<long> NotificationCountLastWeek { get; set; }

        public RecordingsModel RecordingsModel { get; set; }

        public bool ScheduleModeChangeEnabled { get; set; }
    }
}