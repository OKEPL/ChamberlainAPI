namespace Chamberlain.AppServer.Api.Contracts.Services
{
    using System.Collections.Generic;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

    public interface ICustomerEmailService
    {
        void AddEmail(string userName, string email, bool alerts, bool newsletter);

        void DeleteEmail(string userName, string email);

        List<EmailModel> GetEmails(string userName);

        void UpdateEmails(string userName, List<EmailModel> emailModelNewData);
    }
}