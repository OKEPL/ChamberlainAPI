namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Schedule
{
    #region

    using System.ComponentModel.DataAnnotations;

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;

    #endregion

    public class ScheduledMode : BaseChamberlainModel
    {
        public int Duration { get; set; }

        public long Id { get; set; }

        public ModeModel Mode { get; set; }

        [DataType(DataType.Time)]
        public string StartAt { get; set; }
    }
}