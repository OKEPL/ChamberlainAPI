namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class ActivateAccountModel : HasUserNameModel
    {
        [Required]
        [RegularExpression(StaticExpressions.NoGaps)]
        public string Token { get; set; }
    }
}