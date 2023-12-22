namespace Chamberlain.AppServer.Api.Contracts.Commands.Features
{
    public class GetFeatureById
    {
        public GetFeatureById(long featureId)
        {
            this.FeatureId = featureId;
        }

        public long FeatureId { get; }
    }

    public class GetAll : HasUserName
    {
        public GetAll(string userName)
            : base(userName)
        {
        }
    }
}