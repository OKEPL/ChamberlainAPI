namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class PairDeviceModel : BaseChamberlainModel
    {
        [Required]
        public bool ActionToggle { get; set; }

        public long? ThingId { get; set; }
    }
}