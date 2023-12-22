using System;
using System.Linq;
using Chamberlain.Database.Persistency.Model;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Helpers
{
    public static partial class ScheduleHelper
    {

        public static bool SetScheduleActive(string userName, long scheduleId)
        {
            using (var context = new Entities())
            {
                var schedules = context.Customers.Include(x => x.Schedules).First(e => e.Username.Equals(userName)).Schedules;
                var scheduleToActivate = schedules.First(e => e.Id == scheduleId);

                schedules.ToList().ForEach(s => s.IsActive = false);
                scheduleToActivate.IsActive = true;
                context.SaveChanges();

                return true;
            }
        }

        public static bool SetSchedulesInactive(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).First(e => e.Username.Equals(userName));

                customer.Schedules.Where(s => s.IsActive).ToList().ForEach(s => s.IsActive = false);
                context.SaveChanges();
            }

            return true;
        }

        public static NewSchedulesPosition AnalyzeNewSchedulePosition(TimeSpan startTimeEntry, TimeSpan endTimeEntry, TimeSpan startTimeExists, TimeSpan endTimeExists)
        {
            if (startTimeEntry <= startTimeExists && endTimeExists <= endTimeEntry)
                return NewSchedulesPosition.Covers;
            if (startTimeExists <= startTimeEntry && endTimeEntry <= endTimeExists)
                return NewSchedulesPosition.IsInside;
            if (startTimeEntry <= startTimeExists && endTimeEntry <= endTimeExists &&
                startTimeExists <= endTimeEntry)
                return NewSchedulesPosition.OverlapsLeft;
            if (startTimeExists <= startTimeEntry && startTimeEntry <= endTimeExists &&
                endTimeExists <= endTimeEntry)
                return NewSchedulesPosition.OverlapsRight;

            return NewSchedulesPosition.NoAffected;
        }
    }
}