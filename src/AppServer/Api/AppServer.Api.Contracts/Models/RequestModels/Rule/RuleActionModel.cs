using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule
{
    public class RuleActionModel : RuleBaseActionModel
    {   
        [Required]
        public long ItemId { get; set; }
        public string ActionType { get; set; }
        public int ExecutionOrder { get; set; }
        public long DelayInSeconds { get; set; }
        public string ItemValue { get; set; }
        public long ThingId { get; set; }
        public decimal? Increase { get; set; }
        public decimal? Decrease { get; set; }

    }
}