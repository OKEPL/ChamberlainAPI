namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications
{
    using System.ComponentModel.DataAnnotations;
    using Chamberlain.AppServer.Api.Contracts.ValidationAttribute;
    using Chamberlain.Common.Content.Constants;

    [KeyWord(nameof(Ifttt))]
    public class IftttModel : BaseNotificationModel
    {
        [Required]
        public string Ifttt { get; set; }

        public string GetItemType()
        {
            return ItemTypes.Ifttt;
        }
    }
}