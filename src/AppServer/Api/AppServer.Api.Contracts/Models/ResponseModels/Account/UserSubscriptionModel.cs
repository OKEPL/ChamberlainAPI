namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account
{
    public class UserSubscriptionModel : BaseChamberlainModel
    {
        public int Cameras { get; set; }

        public int CamerasMax { get; set; }

        public bool Hub { get; set; }

        public int RecordingsNo { get; set; }

        public bool Sms { get; set; }

        public long Space { get; set; }

        public long SpaceLeft { get; set; }

        public long SubscriptionId { get; set; }

        public string SubscriptionName { get; set; }

        public bool Voip { get; set; }
    }
}