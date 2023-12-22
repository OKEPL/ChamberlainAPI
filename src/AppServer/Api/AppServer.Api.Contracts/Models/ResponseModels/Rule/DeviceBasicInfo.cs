namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule
{
    public class DeviceBasicInfo : BaseChamberlainModel
    {
        public long ThingId { get; set; }
        public string Name { get; set; }
    }
}
