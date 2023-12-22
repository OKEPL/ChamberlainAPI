using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

namespace Chamberlain.AppServer.Api.Contracts.Commands.Customers
{
    public class AddIfttt : HasUserName
    {
        public AddIfttt(string userName, string ifttt, string label, bool alerts)
            : base(userName)
        {
            Ifttt = ifttt;
            Label = label;
            Alerts = alerts;
        }

        public bool Alerts { get; }

        public string Ifttt { get; }

        public string Label { get; }
    }

    public class DeleteIfttt : HasUserName
    {
        public DeleteIfttt(string userName, string ifttt)
            : base(userName)
        {
            Ifttt = ifttt;
        }

        public string Ifttt { get; }
    }

    public class GetIfttt : HasUserName
    {
        public GetIfttt(string userName)
            : base(userName)
        {
        }
    }

    public class UpdateIfttts : HasUserName
    {
        public UpdateIfttts(string userName, List<IftttModel> iftttModelList)
            : base(userName)
        {
            IftttModelList = iftttModelList;
        }

        public List<IftttModel> IftttModelList { get; }
    }
}