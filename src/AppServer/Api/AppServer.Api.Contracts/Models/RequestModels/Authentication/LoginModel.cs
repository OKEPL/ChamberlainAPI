namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Authentication
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public enum PolicyType
    {
        Customer,

        Hub
    }

    public class LoginModel : HasUserNameModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public PolicyType PolicyType { get; set; }
    }
}