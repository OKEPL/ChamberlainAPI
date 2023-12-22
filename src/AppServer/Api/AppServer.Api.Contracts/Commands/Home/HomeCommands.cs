namespace Chamberlain.AppServer.Api.Contracts.Commands.Home
{
    public class GetStatus : HasUserName
    {
        public GetStatus(string userName)
            : base(userName)
        {
        }
    }

    public class ClearEvents : HasUserName
    {
        public ClearEvents(string userName)
            : base(userName)
        {
        }
    }

    public class GetEventsFromBegining : HasUserName
    {
        public GetEventsFromBegining(string userName, int number, string language)
            : base(userName)
        {
            this.Number = number;
            this.Language = language;
        }

        public string Language { get; }

        public int Number { get; }
    }

    public class GetEventsFromName : HasUserName
    {
        public GetEventsFromName(string userName, int number, int lastSeen, string language)
            : base(userName)
        {
            this.Number = number;
            this.LastSeen = lastSeen;
            this.Language = language;
        }

        public string Language { get; }

        public int LastSeen { get; }

        public int Number { get; }
    }

    public class GetCamerasWithImages : HasUserName
    {
        public GetCamerasWithImages(string userName)
            : base(userName)
        {
        }
    }
    
    public class GetCameraImageByThingId : HasUserName
    {
        public GetCameraImageByThingId(string userName, long thingId)
            : base(userName)
        {
            this.ThingId = thingId;
        }

        public long ThingId { get; }
    }

    public class GetNewestEventsFromName : HasUserName
    {
        public GetNewestEventsFromName(string userName, int lastSeen, string language)
            : base(userName)
        {
            this.LastSeen = lastSeen;
            this.Language = language;
        }

        public string Language { get; }

        public int LastSeen { get; }
    }
}