namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Authentication
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    /// <summary>
    /// The nest authentication session model.
    /// </summary>
    public class NestAuthenticationSessionModel : BaseChamberlainModel
    {
        [Required]
        [RegularExpression(StaticExpressions.NoGaps)]
        public string SessionHash { get; set; }
    }
}