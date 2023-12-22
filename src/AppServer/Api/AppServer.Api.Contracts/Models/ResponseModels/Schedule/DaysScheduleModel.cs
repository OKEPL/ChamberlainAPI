namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Schedule
{
    using System.Collections.Generic;
    
    public class DaysScheduleModel : BaseChamberlainModel
    {
        public DaysScheduleModel()
        {
            Days = new List<DayModel>();
        }

        public List<DayModel> Days { get; set; }

        public long Id { get; set; }

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }

        public string Name { get; set; }
    }
}