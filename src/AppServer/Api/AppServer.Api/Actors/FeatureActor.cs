using Chamberlain.AppServer.Api.Contracts.Commands.Features;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class FeatureActor : Receiver
    {
        [SetterProperty]
        public IFeatureService FeatureService { get; set; }

        public FeatureActor()
        {
			Receive<GetAll>(msg => {
				Context.Handle(msg, item => FeatureService.GetAll(item.UserName));
				});
			Receive<GetFeatureById>(msg => {
				Context.Handle(msg, item => FeatureService.GetFeatureById(item.FeatureId));
				});
        }
    }
}
