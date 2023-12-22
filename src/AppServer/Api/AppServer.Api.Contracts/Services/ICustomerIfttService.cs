namespace Chamberlain.AppServer.Api.Contracts.Services
{
    using System.Collections.Generic;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

    public interface ICustomerIfttService
    {
        void AddIfttt(string userName, string ifttt, string label, bool alerts);

        void DeleteIfttt(string userName, string ifttt);

        List<IftttModel> GetIfttt(string userName);

        void UpdateIfttts(string userName, List<IftttModel> iftttModelList);
    }
}