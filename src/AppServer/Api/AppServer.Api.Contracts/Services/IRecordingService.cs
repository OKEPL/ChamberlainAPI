namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using System;
    using System.Collections.Generic;

    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Recordings;

    #endregion

    public interface IRecordingService
    {
        void DeleteList(string userName, string recordingIdList);

        void DeleteRecording(string userName, long recordingId);

        RecordingModel GetRecording(string userName, long recordingId);
        
        List<RecordingModel> GetRecordingsByDate(string userName, string date);

        void MarkRecordingAsSeen(string userName, long recordingId);

        List<DateTime> RecordingDates(string userName);

        RecordingExcludedDatesModel RecordingExcludedDates(string userName);
    }
}