namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion
    
    public class UpdateRecordingBracketsTimeModel : BaseChamberlainModel
    {
        [Required, Range(0, 300)]
        public long? PostRecTime { get; set; }

        [Required, Range(0, 300)]
        public long? PreRecTime { get; set; }
    }
}