namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Items
{
    #region

    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    #endregion

    public class ItemsGroupInModel : BaseChamberlainModel
    {
        public ItemsGroupInModel()
        {
            this.ModeId = 0;
            this.ItemsIds = new List<long>();
            this.MaxGroupDetectionInterval = 0;
        }

        [Required]
        public List<long> ItemsIds { get; set; }

        public int MaxGroupDetectionInterval { get; set; }

        [Required]
        public long ModeId { get; set; }
    }
}