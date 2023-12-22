namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Stb
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class StbLoginModel : HasUserNameModel
    {
        [Required]
        public string SolocooLogin { get; set; }
    }
}