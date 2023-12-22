using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode
{
    public class ModeModel : BaseChamberlainModel
    {
        [Required]
        public long ModeId { get; set; }

        [Required]
        [RegularExpression(StaticExpressions.StringWithNumbersAndGaps)]
        public string Name { get; set; }

        [Required]
        public string Color { get; set; }
    }
}