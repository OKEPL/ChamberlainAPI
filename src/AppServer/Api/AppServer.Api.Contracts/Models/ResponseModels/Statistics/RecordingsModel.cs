namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Statistics
{
    public class RecordingsModel : BaseChamberlainModel
    {
        public long Count { get; set; }

        public long TotalSeconds { get; set; }
    }
}