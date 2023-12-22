namespace Chamberlain.AppServer.Api.Contracts.Commands.Schedules
{
    #region

    using System;

    #endregion

    public class AddSchedule : HasUserName
    {
        public AddSchedule(string userName, string name)
            : base(userName)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class AddScheduleEntry : HasUserName
    {
        public AddScheduleEntry(string userName, long scheduleId, long modeId, int weekDay, TimeSpan startTime, int duration)
            : base(userName)
        {
            ScheduleId = scheduleId;
            ModeId = modeId;
            WeekDay = weekDay;
            StartTime = startTime;
            Duration = duration;
        }

        public long ScheduleId { get; }

        public long ModeId { get; }
        
        public int WeekDay { get; }

        public TimeSpan StartTime { get; }

        public int Duration { get; }
    }

    public class DeleteSchedule : HasUserName
    {
        public DeleteSchedule(string userName, long scheduleId)
            : base(userName)
        {
            ScheduleId = scheduleId;
        }

        public long ScheduleId { get; }
    }

    public class DeleteScheduleEntry : HasUserName
    {
        public DeleteScheduleEntry(string userName, long entryId)
            : base(userName)
        {
            EntryId = entryId;
        }

        public long EntryId { get; }
    }

    public class GetActiveSchedule : HasUserName
    {
        public GetActiveSchedule(string userName)
            : base(userName)
        {
        }
    }

    public class GetSchedules : HasUserName
    {
        public GetSchedules(string userName)
            : base(userName)
        {
        }
    }

    public class GetScheduledModes : HasUserName
    {
        public GetScheduledModes(string userName)
            : base(userName)
        {
        }
    }

    public class SetFirstScheduleActive : HasUserName
    {
        public SetFirstScheduleActive(string userName)
            : base(userName)
        {
        }
    }

    public class SetActiveSchedule : HasUserName
    {
        public SetActiveSchedule(string userName, long scheduleId)
            : base(userName)
        {
            ScheduleId = scheduleId;
        }

        public long ScheduleId { get; }
    }

    public class SetInactiveSchedules : HasUserName
    {
        public SetInactiveSchedules(string userName)
            : base(userName)
        {
        }
    }

    public class UpdateScheduleName : HasUserName
    {
        public UpdateScheduleName(string userName, long scheduleId, string name)
            : base(userName)
        {
            ScheduleId = scheduleId;
            Name = name;
        }

        public string Name { get; }

        public long ScheduleId { get; }
    }
    
    public class UpdateScheduleEntry : HasUserName
    {
        public UpdateScheduleEntry(string userName, long entryId, long scheduleId, long modeId, int weekDay, TimeSpan startTime, int duration)
            : base(userName)
        {
            EntryId = entryId;
            ScheduleId = scheduleId;
            ModeId = modeId;
            WeekDay = weekDay;
            StartTime = startTime;
            Duration = duration;
        }

        public long EntryId { get; }

        public long ScheduleId { get; }

        public long ModeId { get; }

        public int WeekDay { get; }

        public TimeSpan StartTime { get; }
        
        public int Duration { get; }
    }

    public class AddScheduleEntity : HasUserName
    {
        public AddScheduleEntity(string userName, long scheduleId, long modeId, int weekDay, TimeSpan startAt, int duration)
            : base(userName)
        {
            ScheduleId = scheduleId;
            ModeId = modeId;
            WeekDay = weekDay;
            StartAt = startAt;
            Duration = duration;
        }
        
        public int Duration { get; }

        public long ModeId { get; }

        public long ScheduleId { get; }

        public TimeSpan StartAt { get; }

        public int WeekDay { get; }
    }
}