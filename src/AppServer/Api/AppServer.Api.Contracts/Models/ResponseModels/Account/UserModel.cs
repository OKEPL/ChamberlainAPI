namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account
{
    #region

    using System;
    using System.Collections.Generic;

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device;

    #endregion

    public class UserModel : BaseChamberlainModel
    {
        public ModeModel CurrentMode { get; set; }

        public string Email { get; set; }

        public long Id { get; set; }

        public DateTime LastLogin { get; set; }

        public List<SettingModel> Settings { get; set; }

        public string UserName { get; set; }
    }
}