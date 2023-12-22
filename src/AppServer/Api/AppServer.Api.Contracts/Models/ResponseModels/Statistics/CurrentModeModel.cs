namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Statistics
{
    public class CurrentModeModel : BaseChamberlainModel
    {
        public long ModeId { get; set; }

        public string Name { get; set; }
    }
}