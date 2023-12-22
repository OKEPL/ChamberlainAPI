namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications
{
    using System.ComponentModel.DataAnnotations;
    using Chamberlain.AppServer.Api.Contracts.ValidationAttribute;
    using Chamberlain.Common.Content.Constants;

    [KeyWord(nameof(PhoneNumber))]
    public class SecurityPhoneModel : BaseNotificationModel
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(StaticExpressions.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public bool Voip { get; set; }
        public string GetItemType()
        {
            return ItemTypes.Sms;
        }
    }
}