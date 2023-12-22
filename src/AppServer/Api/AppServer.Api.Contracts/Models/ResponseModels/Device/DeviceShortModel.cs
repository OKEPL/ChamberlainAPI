namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device
{
    public class DeviceShortModel : BaseChamberlainModel
    {
        public long ThingId { get; set; }

        public string ThingName { get; set; }

        public DeviceShortModel()
        {
            ThingId = 0;
            ThingName = string.Empty;
        }
    }
}