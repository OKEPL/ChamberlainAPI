using System;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.AppServer.Api.Helpers;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.ExternalServices.RabbitMq;
using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Schedule;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Chamberlain.AppServer.Api.Services
{
    public class ScheduleService : IScheduleService
    {
        public List<ScheduleModel> GetSchedules(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).ThenInclude(x => x.ScheduleEntries).ThenInclude(x =>x.Mode).First(x => x.Username.Equals(userName));

                var result = new List<ScheduleModel>();
                foreach (var schedule in customer.Schedules)
                {
                    var scheduleModel = new ScheduleModel
                    {
                        ScheduleId = schedule.Id,
                        Name = schedule.Name,
                        IsActive = schedule.IsActive,
                        IsDefault = schedule.IsDefault
                    };

                    foreach (var scheduleEntry in schedule.ScheduleEntries)
                    {
                        var entry = new ScheduledModeModel
                        {
                            ScheduleId = scheduleEntry.Id,
                            ModeId = scheduleEntry.ModeId,
                            ModeName = scheduleEntry.Mode.Name,
                            Weekday = scheduleEntry.Weekday,
                            StartTime = scheduleEntry.StartTime,
                            Duration = scheduleEntry.Duration
                        };

                        scheduleModel.ScheduledModes.Add(entry);
                    }

                    scheduleModel.ScheduledModes = scheduleModel.ScheduledModes.OrderBy(x => x.Weekday).ThenBy(x => x.StartTime).ToList();
                    result.Add(scheduleModel);
                }

                return result;
            }
        }

        public List<DaysScheduleModel> GetScheduledModes(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).First(x => x.Username.Equals(userName));

                return customer.Schedules.Select(CreateScheduleResponse).ToList();
            }
        }

        public ScheduleModel GetActiveSchedule(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).ThenInclude(x => x.ScheduleEntries).ThenInclude(x => x.Mode).First(x => x.Username.Equals(userName));

                var schedule = customer.Schedules.Single(s => s.IsActive);

                var scheduleModel = new ScheduleModel
                {
                    ScheduleId = schedule.Id,
                    Name = schedule.Name,
                    IsActive = schedule.IsActive,
                    IsDefault = schedule.IsDefault
                };

                foreach (var scheduleEntry in schedule.ScheduleEntries)
                {
                    var entry = new ScheduledModeModel
                    {
                        ScheduleId = scheduleEntry.Id,
                        ModeId = scheduleEntry.ModeId,
                        ModeName = scheduleEntry.Mode.Name,
                        Weekday = scheduleEntry.Weekday,
                        StartTime = scheduleEntry.StartTime,
                        Duration = scheduleEntry.Duration
                    };

                    scheduleModel.ScheduledModes.Add(entry);
                }

                return scheduleModel;
            }
        }

        public void SetActiveSchedule(string userName, long scheduleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                ScheduleHelper.SetSchedulesInactive(userName);
                ScheduleHelper.SetScheduleActive(userName, scheduleId);

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        public void SetActiveDefaultSchedule(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).First(x => x.Username.Equals(userName));

                var schedule = customer.Schedules.First(s => s.IsDefault);

                ScheduleHelper.SetSchedulesInactive(userName);
                if (!ScheduleHelper.SetScheduleActive(userName, schedule.Id))
                    throw new InvalidOperationException("Could not activate default schedule");

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        public void SetInactiveSchedules(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                if (!ScheduleHelper.SetSchedulesInactive(userName))
                    throw new InvalidOperationException("Could not deactivate schedule");

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        public void AddSchedule(string userName, string name)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                var newSchedule = new Schedule
                {
                    CustomerId = customer.Id,
                    Name = name,
                    IsActive = false,
                    IsDefault = false
                };
                context.Schedules.Add(newSchedule);
                context.SaveChanges();
            }
        }

        public void UpdateScheduleName(string userName, long scheduleId, string name)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).First(x => x.Username.Equals(userName));

                var schedule = customer.Schedules.First(sc => sc.Id == scheduleId);

                schedule.Name = name;
                context.SaveChanges();
            }
        }

        public void DeleteSchedule(string userName, long scheduleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                var schedule = context.Schedules.FirstOrDefault(s => s.Id == scheduleId);
                if (schedule == null)
                    return;

                context.Schedules.Remove(schedule);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        public void AddScheduleEntry(string userName, long scheduleId, long modeId, int weekday, TimeSpan startTime, int duration)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).Include(x => x.Modes)
                    .First(x => x.Username.Equals(userName));
                
                var schedule = customer.Schedules.First(s => s.Id == scheduleId);

                var mode = customer.Modes.First(m => m.Id == modeId);
                
                var scheduleEntry = new ScheduleEntry
                {
                    ScheduleId = schedule.Id,
                    ModeId = mode.Id,
                    Weekday = weekday,
                    StartTime = startTime,
                    Duration = duration
                };
                context.ScheduleEntries.Add(scheduleEntry);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        public void UpdateScheduleEntry(string userName, long entryId, long scheduleId, long modeId, int weekDay, TimeSpan startTime, int duration)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).Include(x => x.Modes).First(x => x.Username.Equals(userName));

                var schedule = customer.Schedules.First(s => s.Id == scheduleId);

                var mode = customer.Modes.First(m => m.Id == modeId);

                var entry = context.ScheduleEntries.First(se => se.Id == entryId);

                entry.ScheduleId = schedule.Id;
                entry.ModeId = mode.Id;
                entry.Weekday = weekDay;
                entry.StartTime = startTime;
                entry.Duration = duration;
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        public void DeleteScheduleEntry(string userName, long entryId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).ThenInclude(x => x.ScheduleEntries).First(x => x.Username.Equals(userName));
                var entry = customer.Schedules.SelectMany(x => x.ScheduleEntries).FirstOrDefault(se => se.Id == entryId);
                if (entry == null)
                    return;
                context.ScheduleEntries.Remove(entry);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        public void ScheduleEntry(string userName, long scheduleId, long modeId, int weekDay, TimeSpan startTime, int duration)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Schedules).ThenInclude(x => x.ScheduleEntries).Include(x => x.Modes).First(x => x.Username.Equals(userName));

                var schedule = customer.Schedules.First(s => s.Id == scheduleId);

                var mode = customer.Modes.First(m => m.Id == modeId);

                var newEntry = new ScheduleEntry
                {
                    ScheduleId = schedule.Id,
                    ModeId = mode.Id,
                    Weekday = weekDay,
                    StartTime = startTime,
                    Duration = duration
                };

                var newEntryEndTime = startTime.Add(TimeSpan.FromMinutes(duration));
                var scheduleEntriesOverlapped = schedule.ScheduleEntries
                    .Where(se => se.Weekday == weekDay)
                    .Where(se => se.StartTime >= startTime && se.StartTime <= newEntryEndTime || 
                                 se.StartTime.Add(TimeSpan.FromMinutes(se.Duration)) >= startTime && se.StartTime.Add(TimeSpan.FromMinutes(se.Duration)) <= newEntryEndTime ||
                                 startTime >= se.StartTime && newEntryEndTime <= se.StartTime.Add(TimeSpan.FromMinutes(se.Duration)))
                    .Distinct()
                    .ToList();

                newEntry = AdjustNewScheduleEntry(modeId, scheduleEntriesOverlapped, newEntry, context);
                context.ScheduleEntries.Add(newEntry);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        private static DaysScheduleModel CreateScheduleResponse(Schedule customerSchedule)
        {
            var schedule = new DaysScheduleModel
            {
                Id = customerSchedule.Id,
                Name = customerSchedule.Name,
                IsActive = customerSchedule.IsActive,
                IsDefault = customerSchedule.IsDefault
            };

            for (var i = 0; i < 7; i++)
            {
                var weekDay = new DayModel
                {
                    Id = i
                };

                var schedulesEntries = customerSchedule.ScheduleEntries
                    .Where(se => se.Weekday == weekDay.Id)
                    .OrderBy(se => se.StartTime);

                foreach (var scheduleEntry in schedulesEntries)
                    weekDay.ScheduledModes.Add(CreateScheduledModeResponse(scheduleEntry));

                schedule.Days.Add(weekDay);
            }

            return schedule;
        }

        private static ScheduledMode CreateScheduledModeResponse(ScheduleEntry scheduleEntry)
        {
            return new ScheduledMode
            {
                Id = scheduleEntry.Id,
                Mode = new ModeModel
                {
                    ModeId = scheduleEntry.Mode.Id,
                    Name = scheduleEntry.Mode.Name,
                    Color = $"#{scheduleEntry.Mode.Color}"
                },

                StartAt = $"{scheduleEntry.StartTime.Hours:00}:{scheduleEntry.StartTime.Minutes:00}",
                Duration = scheduleEntry.Duration
            };
        }

        private ScheduleEntry AdjustNewScheduleEntry(long modeId, IEnumerable<ScheduleEntry> schedulesEntriesOverlapped,
            ScheduleEntry newEntry, Entities context)
        {
            var newEntryEndTime = newEntry.StartTime.Add(TimeSpan.FromMinutes(newEntry.Duration));
            newEntryEndTime = ShrinkIfOverextendsOneDay(newEntry, newEntryEndTime);

            foreach (var existingSchedule in schedulesEntriesOverlapped)
            {
                var scheduleEndTime =
                    existingSchedule.StartTime.Add(TimeSpan.FromMinutes(existingSchedule.Duration));

                switch (ScheduleHelper.AnalyzeNewSchedulePosition(newEntry.StartTime, newEntryEndTime,
                    existingSchedule.StartTime, scheduleEndTime))
                {
                    case ScheduleHelper.NewSchedulesPosition.Covers:
                        RemoveScheduleEntry(context, existingSchedule);
                        break;
                    case ScheduleHelper.NewSchedulesPosition.OverlapsLeft:
                        if (SameModeScheduleExtendDuration(modeId, newEntry, context, existingSchedule, scheduleEndTime, newEntryEndTime))
                            break;

                        CutDurationScheduleFromLeft(existingSchedule, newEntryEndTime);
                        break;
                    case ScheduleHelper.NewSchedulesPosition.OverlapsRight:
                        if (SameModeScheduleExtendDuration(modeId, newEntry, context, existingSchedule, newEntry.StartTime, existingSchedule.StartTime))
                            break;

                        CutDurationScheduleFromRight(newEntry, existingSchedule, scheduleEndTime);
                        break;
                    case ScheduleHelper.NewSchedulesPosition.IsInside:
                        if (existingSchedule.ModeId == modeId)
                        {
                            RemoveScheduleEntry(context, existingSchedule);
                            break;
                        }

                        AddNewEntrySplittedSchedule(newEntry, context, existingSchedule, scheduleEndTime, newEntryEndTime);
                        break;
                    case ScheduleHelper.NewSchedulesPosition.NoAffected:
                        break;
                }
            }

            return newEntry;
        }

        private static TimeSpan ShrinkIfOverextendsOneDay(ScheduleEntry newEntry, TimeSpan newEntryEndTime)
        {
            if (newEntryEndTime.Days > 0)
                newEntryEndTime = TimeSpan.FromDays(1);

            newEntry.Duration = (int)(newEntryEndTime - newEntry.StartTime).TotalMinutes;
            return newEntryEndTime;
        }

        private static void AddNewEntrySplittedSchedule(ScheduleEntry newEntry, Entities context, ScheduleEntry existingSchedule,
            TimeSpan scheduleEndTime, TimeSpan newEntryEndTime)
        {
            if (existingSchedule.StartTime != newEntry.StartTime && scheduleEndTime != newEntryEndTime)
                context.ScheduleEntries.Add(new ScheduleEntry
                {
                    ScheduleId = existingSchedule.ScheduleId,
                    ModeId = existingSchedule.ModeId,
                    Weekday = existingSchedule.Weekday,
                    StartTime = newEntryEndTime,
                    Duration = (int) (scheduleEndTime - newEntryEndTime).TotalMinutes
                });

            existingSchedule.Duration = (int) (newEntry.StartTime - existingSchedule.StartTime)
                .TotalMinutes;
        }

        private static void CutDurationScheduleFromRight(ScheduleEntry newEntry, ScheduleEntry existingSchedule, TimeSpan scheduleEndTime)
        {
            existingSchedule.Duration += (int) (newEntry.StartTime - scheduleEndTime)
                .TotalMinutes;
        }

        private static void CutDurationScheduleFromLeft(ScheduleEntry existingSchedule, TimeSpan newEntryEndTime)
        {
            existingSchedule.Duration += (int) (existingSchedule.StartTime - newEntryEndTime).TotalMinutes;
            existingSchedule.StartTime = newEntryEndTime;
        }

        private static bool SameModeScheduleExtendDuration(long modeId, ScheduleEntry newEntry, Entities context,
            ScheduleEntry existingSchedule, TimeSpan scheduleEndTime, TimeSpan newEntryEndTime)
        {
            if (existingSchedule.ModeId != modeId) return false;

            if (newEntry.StartTime > existingSchedule.StartTime)
                newEntry.StartTime = existingSchedule.StartTime;

            RemoveScheduleEntry(context, existingSchedule);
            newEntry.Duration += (int) (scheduleEndTime - newEntryEndTime).TotalMinutes;

            return true;
        }

        private static void RemoveScheduleEntry(Entities context, ScheduleEntry existingSchedule)
        {
            context.ScheduleEntries.Remove(existingSchedule);
        }
    }
}