namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications
{
    #region

    using System.Collections.Generic;

    using Chamberlain.AppServer.Api.Contracts.ModelBinder;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    [ModelBinder(BinderType = typeof(NotificationModelBinder))]
    public class UpdateAccountNotificationsModel : BaseChamberlainModel
    {
        public List<BaseNotificationModel> Notifications { get; set; }
    }
}