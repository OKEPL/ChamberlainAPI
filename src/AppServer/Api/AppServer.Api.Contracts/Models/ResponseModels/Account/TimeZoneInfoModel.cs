using Common.StaticMethods.StaticMethods;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account
{
    public class TimeZoneInfoModel : BaseChamberlainModel
    {
        public TimeZoneInfoModel()
        {
            this.Id = TimeZoneInfoExtended.DefaultTimeZone;
            this.TimezoneName = string.Empty;
            this.TimezoneType = string.Empty;
        }

        public int Id { get; set; }

        public string TimezoneName { get; set; }

        public string TimezoneType { get; set; }
    }
}