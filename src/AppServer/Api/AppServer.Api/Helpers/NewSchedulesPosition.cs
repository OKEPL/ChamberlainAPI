namespace Chamberlain.AppServer.Api.Helpers
{
    public static partial class ScheduleHelper
    {
        public enum NewSchedulesPosition
        {
            IsInside,
            OverlapsLeft,
            OverlapsRight,
            Covers,
            NoAffected
        }
    }
}