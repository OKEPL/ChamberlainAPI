namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Feature
{
    public class FeatureModel : BaseChamberlainModel
    {
        public int Cameras { get; set; }

        public bool Current { get; set; }

        public string Description { get; set; }

        public long DiskSpace { get; set; }

        public int FeatureType { get; set; }

        public bool Hub { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public int PriceCen { get; set; }

        public int PriceEur { get; set; }

        public bool Sms { get; set; }

        public bool Voip { get; set; }
    }
}