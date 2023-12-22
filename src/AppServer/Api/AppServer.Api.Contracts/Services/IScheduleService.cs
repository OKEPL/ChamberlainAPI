using System;
using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Schedule;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Schedule;

namespace Chamberlain.AppServer.Api.Contracts.Services
{
    public interface IScheduleService
    {
        void AddSchedule(string userName, string name);

        void AddScheduleEntry(string userName, long scheduleId, long modeId, int weekday, TimeSpan startTime, int duration);

        void DeleteSchedule(string userName, long scheduleId);

        void DeleteScheduleEntry(string userName, long entryId);

        ScheduleModel GetActiveSchedule(string userName);

        List<DaysScheduleModel> GetScheduledModes(string userName);

        List<ScheduleModel> GetSchedules(string userName);

        void ScheduleEntry(string userName, long scheduleId, long modeId, int weekDay, TimeSpan startTime, int duration);

        void SetActiveDefaultSchedule(string userName);

        void SetActiveSchedule(string userName, long scheduleId);

        void SetInactiveSchedules(string userName);

        void UpdateScheduleEntry(string userName, long entryId, long scheduleId, long modeId, int weekDay, TimeSpan startTime, int duration);

        void UpdateScheduleName(string userName, long scheduleId, string name);
    }
}