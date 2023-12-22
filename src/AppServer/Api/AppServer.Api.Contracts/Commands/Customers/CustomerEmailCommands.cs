using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

namespace Chamberlain.AppServer.Api.Contracts.Commands.Customers
{
    public class AddEmail : HasUserName
    {
        public AddEmail(string userName, string email, bool alerts, bool newsletter)
            : base(userName)
        {
            Email = email;
            Alerts = alerts;
            Newsletter = newsletter;
        }

        public bool Alerts { get; }

        public string Email { get; }

        public bool Newsletter { get; }
    }

    public class DeleteEmail : HasUserName
    {
        public DeleteEmail(string userName, string email)
            : base(userName)
        {
            Email = email;
        }

        public string Email { get; }
    }

    public class GetEmails : HasUserName
    {
        public GetEmails(string userName)
            : base(userName)
        {
        }
    }

    public class UpdateEmails : HasUserName
    {
        public UpdateEmails(string userName, List<EmailModel> emailModelList)
            : base(userName)
        {
            EmailModelList = emailModelList;
        }

        public List<EmailModel> EmailModelList { get; }
    }
}