using System;
using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Schedule
{
    public class ScheduledModeModel : BaseChamberlainModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public long ScheduleId { get; set; }

        [Required]
        public long ModeId { get; set; }

        [Required]
        [RegularExpression(StaticExpressions.StringWithNumbersAndGaps)]
        public string ModeName { get; set; }

        [Required]
        public int Weekday { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public int Duration { get; set; }
    }
}