namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class ChangePasswordModel : BaseChamberlainModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}