namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Event
{
    #region

    using System;

    #endregion

    public class EventModel : BaseChamberlainModel
    {
        public int code { get; set; }

        public DateTime date { get; set; }

        public long id { get; set; }

        public string itemName { get; set; }

        public string payload { get; set; }

        public long secondsAgo { get; set; }

        public string type { get; set; }
    }
}