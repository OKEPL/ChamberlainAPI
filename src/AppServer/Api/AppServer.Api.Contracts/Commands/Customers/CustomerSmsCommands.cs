using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

namespace Chamberlain.AppServer.Api.Contracts.Commands.Customers
{
    public class AddSms : HasUserName
    {
        public AddSms(string userName, string sms, string label, bool alerts, bool voip)
            : base(userName)
        {
            Sms = sms;
            Label = label;
            Alerts = alerts;
            Voip = voip;
        }

        public bool Alerts { get; }

        public string Label { get; }

        public string Sms { get; }

        public bool Voip { get; }
    }

    public class DeleteSms : HasUserName
    {
        public DeleteSms(string userName, string sms)
            : base(userName)
        {
            Sms = sms;
        }

        public string Sms { get; }
    }

    public class GetSms : HasUserName
    {
        public GetSms(string userName)
            : base(userName)
        {
        }
    }

    public class UpdateSmses : HasUserName
    {
        public UpdateSmses(string userName, List<SmsModel> smsModelList)
            : base(userName)
        {
            SmsModelList = smsModelList;
        }

        public List<SmsModel> SmsModelList { get; }
    }

    public class UpdateSecurityPhones : HasUserName
    {
        public UpdateSecurityPhones(string userName, List<SecurityPhoneModel> smsModelList)
            : base(userName)
        {
            SecurityPhoneModelList = smsModelList;
        }

        public List<SecurityPhoneModel> SecurityPhoneModelList { get; }
    }
}