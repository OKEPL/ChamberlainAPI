using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts;

namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Authentication;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account;
    using System.Collections.Generic;

    #endregion

    public interface ICustomerService
    {
        void AddUser(string name, string email, string password, IActorRef ruleEngineActorRef);

        CustomerAddressModel GetAddress(string userName);

        void AssignAddress(string userName, CustomerAddressBaseModel data);

        void ChangeTimezone(string userName, int timezone);

        void ChangeUserMode(string userName, long modeId, IActorRef ruleEngineActiorRef);

        void ChangeUserSubscription(string userName, long featureId);

        NestAuthenticationSessionModel CreateNestAuthenticationSession(string userName, string redirectTo);

        NestRedirectionModel GetNestTokenAndRedirectBack(string state, string code);

        void DiscardNestConnection(string userName);

        AccountDataModel GetAccountData(string userName);

        UserModel GetUser(string userName);

        UserSubscriptionModel GetUserSubscription(string userName);

        void RestorePassword(string userName, string password);

        void StartRestorePassword(string name);

        void UpdateAddress(string userName, CustomerAddressBaseModel data);

        void UpdateNotification(string userName, UpdateAccountNotificationsModel data);

        void UpdateRecordingBracketsTime(string userName, long preRecTime, long postRecTime);

        List<TimeZoneInfoModel> GetTimezones();
    }
}