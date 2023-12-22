namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications
{
    #region

    using System.ComponentModel.DataAnnotations;

    using Chamberlain.AppServer.Api.Contracts.ValidationAttribute;
    using Chamberlain.Common.Content.Constants;

    #endregion

    [KeyWord("Firebase")]
    public class FirebasePushModel : BaseNotificationModel
    {
        [Required]
        public string Firebase { get; set; }

        public string GetItemType()
        {
            return ItemTypes.MobileFirebasePush;
        }
    }
}