using System;
using System.Collections.Generic;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Statistics;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Domotica;
using Chamberlain.Database.Persistency.Model;
using Common.StaticMethods.StaticMethods;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Chamberlain.AppServer.Api.Services
{
    public class StatisticsService : IStatisticsService
    {
        public StatisticsModel GetStatistics(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Recordings).First(x => x.Username.Equals(userName));

                var statistics = new StatisticsModel
                {
                    RecordingsModel = GetRecordingsData(customer),
                    CurrentModeModel = GetCurrentModeData(customer),
                    FreeSpaceModel = GetFreeSpaceData(customer),
                    NotificationCountLastWeek = GetNotificationCountFromLastWeek(customer),
                    ScheduleModeChangeEnabled = customer.Schedules.Any(x => x.IsActive)
                };
                return statistics;
            }
        }

        private static DateTime CustomerCurrentDay(Customer customer)
        {
            return StaticMethods.ConvertToCustomerTimeZone(DateTime.UtcNow, customer.Timezone).Date;
        }

        private static RecordingsModel GetRecordingsData(Customer customer)
        {
            return new RecordingsModel
            {
                Count = customer.Recordings.Count,
                TotalSeconds = customer.Recordings.Sum(x => x.Duration)
            };
        }

        private static CurrentModeModel GetCurrentModeData(Customer customer)
        {
            return new CurrentModeModel
            {
                ModeId = customer.CurrentMode?.Id ?? 0,
                Name = customer.CurrentMode?.Name ?? "None"
            };
        }

        private static FreeSpaceModel GetFreeSpaceData(Customer customer)
        {
            var featureDetails =
                XmlSerialization.FromString<FeatureDetails>(customer.CustomerFeatureBindings.FirstOrDefault(fb => fb.StartDate < DateTime.UtcNow && (!fb.EndDate.HasValue || fb.EndDate.Value > DateTime.UtcNow))?.Feature
                    ?.FeatureDetails);
            if (featureDetails == null)
            {
                Log.Debug($"Unable to fetch FeatureDetails for CustomerID: {customer.Id}");
                return new FreeSpaceModel
                {
                    Megabytes = 0,
                    Percent = 0
                };
            }

            var megabytesLeft = (featureDetails.DiskSpace - customer.Recordings.Sum(x => x.Size)) / (1024 * 1024);
            megabytesLeft = megabytesLeft >= 0 ? megabytesLeft : 0;

            return new FreeSpaceModel
            {
                Megabytes = megabytesLeft,
                Percent = (int) (megabytesLeft * 100 / (featureDetails.DiskSpace / (1024 * 1024)))
            };
        }

        private static IEnumerable<long> GetNotificationCountFromLastWeek(Customer customer)
        {
            return GetNotificationCountForLastDays(customer, 7);
        }

        private static IEnumerable<long> GetNotificationCountForLastDays(Customer customer, int numberOfDays)
        {
            var currentDay = CustomerCurrentDay(customer);
            var startDay = currentDay.Subtract(TimeSpan.FromDays(numberOfDays - 1));
            var events = customer.Events
                .Where(x => StaticMethods.ConvertToCustomerTimeZone(x.DateTime, customer.Timezone).Date > startDay).ToList();
            var days = new List<long>();
            for (var day = startDay; day <= currentDay; day = day.AddDays(1))
                days.Add(events.Count(x => StaticMethods.ConvertToCustomerTimeZone(x.DateTime, customer.Timezone).Date == day));

            return days;
        }
    }
}