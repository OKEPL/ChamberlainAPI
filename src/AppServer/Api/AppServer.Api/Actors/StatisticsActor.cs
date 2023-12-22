using Chamberlain.AppServer.Api.Contracts.Commands.Statistics;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class StatisticsActor : Receiver
    {
        [SetterProperty]
        public IStatisticsService StatisticsService { get; set; }

        public StatisticsActor()
        {
			Receive<GetStatistics>(msg => {
				Context.Handle(msg, item => StatisticsService.GetStatistics(item.UserName));
				});
        }
    }
}