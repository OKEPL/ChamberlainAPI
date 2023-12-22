namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class ChangePinModel : BaseChamberlainModel
    {
        [Required]
        [Range(0, 9999)]
        public int NewPin { get; set; }

        [Required]
        [Range(0, 9999)]
        public int OldPin { get; set; }
    }
}