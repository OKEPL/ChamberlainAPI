namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device
{
    public class SettingModel : BaseChamberlainModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}