namespace Chamberlain.AppServer.Api.Contracts.Commands.Statistics
{
    public class GetStatistics : HasUserName
    {
        public GetStatistics(string userName)
            : base(userName)
        {
        }
    }
}