namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Statistics
{
    public class FreeSpaceModel : BaseChamberlainModel
    {
        public long Megabytes { get; set; }

        public int Percent { get; set; }
    }
}