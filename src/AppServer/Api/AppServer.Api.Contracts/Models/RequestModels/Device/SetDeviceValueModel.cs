namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class SetDeviceValueModel : BaseChamberlainModel
    {
        [Required]
        public long ItemId { get; set; }
                
        public string Value { get; set; }
    }
}