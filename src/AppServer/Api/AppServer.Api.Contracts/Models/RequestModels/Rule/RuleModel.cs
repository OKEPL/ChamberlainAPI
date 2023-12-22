using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule
{
    public class RuleModel : BaseChamberlainModel
    {
        [Required]
        public long RuleId { get; set; }

        [Required]
        public string RuleName { get; set; }

        [Required]
        public bool EmailAlerts { get; set; }

        [Required]
        public bool IftttAlerts { get; set; }

        [Required]
        public bool SmsAlerts { get; set; }

        [Required]
        public bool VoipAlerts { get; set; }

        [Required]
        public bool Warnings { get; set; }
    }
}
