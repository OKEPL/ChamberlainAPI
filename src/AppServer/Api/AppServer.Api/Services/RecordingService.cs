using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Database.Persistency.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chamberlain.Common.Extensions;
using Common.StaticMethods.StaticMethods;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Services
{
    using AppServer.Api.Contracts.Models.ResponseModels.Recordings;

    public class RecordingService : IRecordingService
    {
        public List<RecordingModel> GetRecordingsByDate(string userName, string date)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Recordings).Include(x => x.CustomerFeatureBindings).ThenInclude(x => x.Feature).Include(x => x.Recordings).ThenInclude(x => x.Action).ThenInclude(x => x.Rule).Include(x => x.Recordings).ThenInclude(x => x.Action).ThenInclude(x => x.Item).First(x => x.Username.Equals(userName));
                
                var dateTime = DateTime.Parse(date);
                return customer.Recordings.Where(r => StaticMethods.ConvertToCustomerTimeZone(r.DateTime, customer.Timezone).Date == dateTime.Date)
                    .Select(recording => GetRecordingModel(recording, customer)).ToList();
            }
        }

        public RecordingModel GetRecording(string userName, long recordingId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Recordings).ThenInclude(x => x.Action).ThenInclude(x => x.Item).Include(x => x.Recordings).ThenInclude(x => x.Action).ThenInclude(x => x.Rule).First(x => x.Username.Equals(userName));
                
                var recording = customer.Recordings.Single(x => x.Id == recordingId);
                return GetRecordingModel(recording, customer);
            }
        }

        public void MarkRecordingAsSeen(string userName, long recordingId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Recordings).First(x => x.Username.Equals(userName));
                
                var recording = customer.Recordings.FirstOrDefault(bm => bm.Id == recordingId);
                if (recording != null)
                {
                    recording.Watched = true;
                    context.SaveChanges();
                }
            }
        }

        public void DeleteList(string userName, string recordingIdList)
        {
            var stringIdList = recordingIdList.Split(',');
            foreach (var s in stringIdList)
            {
                if (!long.TryParse(s, out long id))
                    continue;

                DeleteRecording(userName, id);
            }
        }

        public List<DateTime> RecordingDates(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Recordings).First(x => x.Username.Equals(userName));
                return customer.Recordings
                    .OrderBy(r => r.DateTime)
                    .Select(rec => StaticMethods.ConvertToCustomerTimeZone(rec.DateTime, customer.Timezone).Date)
                    .Distinct().ToList();
            }
        }

        public RecordingExcludedDatesModel RecordingExcludedDates(string userName)
        {
            var recordingsDatesList = RecordingDates(userName);
            if (recordingsDatesList.Count == 0)
                return null;
            return new RecordingExcludedDatesModel
            {
                StartDate = recordingsDatesList.FirstOrDefault().ToString("yyyy-MM-dd"),
                EndDate = recordingsDatesList.LastOrDefault().ToString("yyyy-MM-dd"),
                ExcludedDates = recordingsDatesList.ExcludedDates().Select(date => date.ToString("yyyy-MM-dd")).ToList()
            };
        }

        private static RecordingModel GetRecordingModel(Recording recording, Customer customer)
        {
            return new RecordingModel
            {
                id = recording.Id,
                duration = recording.Duration,
                size = recording.Size,
                watched = recording.Watched,
                dateTime = StaticMethods.ConvertToCustomerTimeZone(recording.DateTime, customer.Timezone),
                itemName = recording.ActionId.HasValue ? recording.Action.Item.CustomName : string.Empty,
                recordingUrl = StaticMethods.GetRecordingStreamUrl(recording),
                thumbnailUrl = StaticMethods.GetRecordingThumbUrl(recording),
                description = GetRecordingDescription(recording),
                recording_status = recording.RecordingStatus ?? string.Empty
            };
        }

        public void DeleteRecording(string userName, long recordingId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Recordings).First(x => x.Username.Equals(userName));

                var recording = customer.Recordings.FirstOrDefault(x => x.Id == recordingId);

                if (recording == null)
                    return;

                var filePath = StaticMethods.GetRecordingFilePath(recording);
                var thumbPath = StaticMethods.GetRecordingThumbFilePath(recording);

                context.Recordings.RemoveRange(context.Recordings.Where(i => i.Id == recordingId));
                context.SaveChanges();

                if (File.Exists(filePath))
                    File.Delete(filePath);

                if (File.Exists(thumbPath))
                    File.Delete(thumbPath);
            }
        }

        private static string GetRecordingDescription(Recording recording)
        {
            return !string.IsNullOrEmpty(recording.Description) ? recording.Description : 
                "Triggered by " + (recording.ActionId.HasValue ? recording.Action.Rule.Name : "deleted rule");
        }
    }
}