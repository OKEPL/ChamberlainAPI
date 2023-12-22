namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Schedule
{
    using System.ComponentModel.DataAnnotations;
    
    public class ScheduledModeEntryModel : BaseChamberlainModel
    {
        [Required]
        [Range(1, 1440)]
        public int Duration { get; set; }

        [Required]
        public long ModeId { get; set; }

        [Required]
        public long ScheduleId { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string StartAt { get; set; }

        [Required]
        [Range(0, 6)]
        public int WeekDay { get; set; }
    }
}