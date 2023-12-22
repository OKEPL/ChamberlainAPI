using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Schedule
{
    public class ScheduleModel : BaseChamberlainModel
    {
        public long ScheduleId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }

        [Required]
        [RegularExpression(StaticExpressions.StringWithNumbersAndGaps)]
        public string Name { get; set; }

        public List<ScheduledModeModel> ScheduledModes { get; set; }

        public ScheduleModel()
        {
            ScheduledModes = new List<ScheduledModeModel>();
        }
    }
}