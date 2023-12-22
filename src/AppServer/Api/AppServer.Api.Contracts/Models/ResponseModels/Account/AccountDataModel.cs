using Common.StaticMethods.StaticMethods;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account
{
    #region

    using System.Collections.Generic;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

    #endregion

    public class AccountDataModel : BaseChamberlainModel
    {
        public AccountDataModel()
        {
            this.emailList = new List<EmailModel>();
            this.smsList = new List<SmsModel>();
            this.securityList = new List<SecurityPhoneModel>();
            this.iftttList = new List<IftttModel>();
            this.timezone = TimeZoneInfoExtended.DefaultTimeZone;
            this.subscription = new UserSubscriptionModel();
            this.isLastSubscription = true;
            this.nextSubscription = new UserSubscriptionModel();
            this.preRecordingTime = 0;
            this.postRecordingTime = 0;
            this.postRecordingTime = 0;
        }

        public List<EmailModel> emailList { get; set; }

        public List<IftttModel> iftttList { get; set; }

        public bool isLastSubscription { get; set; }

        public UserSubscriptionModel nextSubscription { get; set; }

        public long postRecordingTime { get; set; }

        public long preRecordingTime { get; set; }

        public List<SmsModel> smsList { get; set; }

        public List<SecurityPhoneModel> securityList { get; set; }

        public UserSubscriptionModel subscription { get; set; }

        public int timezone { get; set; }

        public bool nestConnectionStatus { get; set; }
    }
}