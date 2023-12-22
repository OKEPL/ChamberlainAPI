namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Schedule
{
    using System.Collections.Generic;
    
    public class DayModel : BaseChamberlainModel
    {
        public DayModel()
        {
            ScheduledModes = new List<ScheduledMode>();
        }

        public int Id { get; set; }

        public List<ScheduledMode> ScheduledModes { get; set; }
    }
}