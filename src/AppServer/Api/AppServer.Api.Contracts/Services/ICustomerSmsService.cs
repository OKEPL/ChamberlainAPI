namespace Chamberlain.AppServer.Api.Contracts.Services
{
    using System.Collections.Generic;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

    public interface ICustomerSmsService
    {
        void AddSms(string userName, string sms, string label, bool alerts, bool voip);

        void DeleteSms(string userName, string sms);

        List<SmsModel> GetSms(string userName);

        void UpdateSmses(string userName, List<SmsModel> smsModelList);

        void UpdateSecurityPhones(string userName, List<SecurityPhoneModel> securityPhoneModelList);
    }
}