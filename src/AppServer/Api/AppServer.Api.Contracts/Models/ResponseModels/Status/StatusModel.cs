namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Status
{
    #region

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;

    #endregion

    public class StatusModel : BaseChamberlainModel
    {
        public long cameraMovement { get; set; }

        public ModeModel mode { get; set; }

        public ModeModel modeBySchedule { get; set; }

        public NextModeModel nextSchedChange { get; set; }

        public string status { get; set; }

        public string statusCode { get; set; }
    }
}