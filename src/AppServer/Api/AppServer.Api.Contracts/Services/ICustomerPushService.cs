namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using System.Collections.Generic;

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

    #endregion

    public interface ICustomerPushService
    {
        void AddFirebasePush(string userName, string firebase, string label, bool alerts);

        void DeleteFirebasePush(string userName, string firebase);

        List<FirebasePushModel> GetFirebasePush(string userName);
    }
}