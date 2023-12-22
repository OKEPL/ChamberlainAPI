namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class ChangeTimezoneModel : BaseChamberlainModel
    {
        [Required]
        public int Timezone { get; set; }
    }
}