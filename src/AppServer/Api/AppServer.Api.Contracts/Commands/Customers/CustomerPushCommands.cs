namespace Chamberlain.AppServer.Api.Contracts.Commands.Customers
{
    public class AddFirebasePush : HasUserName
    {
        public AddFirebasePush(string userName, string firebase, string label, bool alerts)
            : base(userName)
        {
            this.Firebase = firebase;
            this.Label = label;
            this.Alerts = alerts;
        }

        public bool Alerts { get; }

        public string Firebase { get; }

        public string Label { get; }
    }

    public class DeleteFirebasePush : HasUserName
    {
        public DeleteFirebasePush(string userName, string firebase)
            : base(userName)
        {
            this.Firebase = firebase;
        }

        public string Firebase { get; }
    }

    public class GetFirebasePush : HasUserName
    {
        public GetFirebasePush(string userName)
            : base(userName)
        {
        }
    }
}