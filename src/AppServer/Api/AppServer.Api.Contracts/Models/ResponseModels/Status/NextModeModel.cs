namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Status
{
    #region

    using System;

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;

    #endregion

    public class NextModeModel : BaseChamberlainModel
    {
        public bool isScheduled { get; set; }

        public ModeModel mode { get; set; }

        public DateTime time { get; set; }
    }
}