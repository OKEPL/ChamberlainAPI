using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule
{
    public class RuleTriggerModel : RuleBaseTriggerModel
    {
        [Required]
        public int MaxConfirmationInterval { get; set; }

        [Required]
        public int ExecutionOrder { get; set; }
        
        public long? ItemId { get; set; }

        public string TriggerType { get; set; }

        public string ItemValue { get; set; }

        public decimal? GreaterThan { get; set; }

        public decimal? LowerThan { get; set; }

        [Required]
        public long MinimalLastTime { get; set; }

        public long? ThingId { get; set; }
    }
}