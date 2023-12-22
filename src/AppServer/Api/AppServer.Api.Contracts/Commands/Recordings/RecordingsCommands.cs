namespace Chamberlain.AppServer.Api.Contracts.Commands.Recordings
{
    public class GetRecordingsByDate : HasUserName
    {
        public GetRecordingsByDate(string userName, string date)
            : base(userName)
        {
            Date = date;
        }

        public string Date { get; set; }
    }

    public class GetRecording : HasUserName
    {
        public GetRecording(string userName, long recordingId)
            : base(userName)
        {
            RecordingId = recordingId;
        }

        public long RecordingId { get; set; }
    }

    public class MarkRecordingAsSeen : HasUserName
    {
        public MarkRecordingAsSeen(string userName, long recordingId)
            : base(userName)
        {
            RecordingId = recordingId;
        }

        public long RecordingId { get; set; }
    }

    public class DeleteRecording : HasUserName
    {
        public DeleteRecording(string userName, long recordingId)
            : base(userName)
        {
            RecordingId = recordingId;
        }

        public long RecordingId { get; set; }
    }

    public class DeleteRecordingList : HasUserName
    {
        public DeleteRecordingList(string userName, string idList)
            : base(userName)
        {
            IdList = idList;
        }

        public string IdList { get; set; }
    }

    public class GetRecordingDates : HasUserName
    {
        public GetRecordingDates(string userName)
            : base(userName)
        {
        }
    }

    public class GetRecordingExcludedDates : HasUserName
    {
        public GetRecordingExcludedDates(string userName)
            : base(userName)
        {
        }
    }
}