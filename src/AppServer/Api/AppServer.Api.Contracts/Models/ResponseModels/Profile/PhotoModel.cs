namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Profile
{
    public class PhotoModel
    {
        public long CustomerId { get; set; }
        public long ProfileId { get; set; }
        public int ModelLabel { get; set; }
        public string FileName { get; set; }
    }
}
