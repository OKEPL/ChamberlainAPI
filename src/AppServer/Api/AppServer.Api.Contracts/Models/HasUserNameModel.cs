namespace Chamberlain.AppServer.Api.Contracts.Models
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class HasUserNameModel : BaseChamberlainModel
    {
        [Required]
        [RegularExpression(StaticExpressions.UserNameConvention)]
        public string UserName { get; set; }
    }
}