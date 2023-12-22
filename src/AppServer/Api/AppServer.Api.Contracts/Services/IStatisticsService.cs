namespace Chamberlain.AppServer.Api.Contracts.Services
{
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Statistics;

    public interface IStatisticsService
    {
        StatisticsModel GetStatistics(string userName);
    }
}