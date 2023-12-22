namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class VerifyPinModel : BaseChamberlainModel
    {
        [Required]
        public string Pin { get; set; }
    }
}