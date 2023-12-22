namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications
{
    using Chamberlain.AppServer.Api.Contracts.ValidationAttribute;

    [KeyWord("Empty")]
    public class BaseNotificationModel : BaseChamberlainModel
    {
        public bool Alerts { get; set; }

        public string Label { get; set; }
    }
}