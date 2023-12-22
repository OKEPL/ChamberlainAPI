namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class AddUserModel : HasUserNameModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}