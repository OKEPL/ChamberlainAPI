namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications
{
    using System.ComponentModel.DataAnnotations;
    using Chamberlain.AppServer.Api.Contracts.ValidationAttribute;
    using Chamberlain.Common.Content.Constants;

    [KeyWord(nameof(Email))]
    public class EmailModel : BaseNotificationModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool Newsletters { get; set; }

        public string GetItemType()
        {
            return ItemTypes.Email;
        }
    }
}