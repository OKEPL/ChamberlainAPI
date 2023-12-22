using Chamberlain.AppServer.Api.Contracts.Commands.Ifttts;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class IftttActor : Receiver
    {
        [SetterProperty]
        public IIftttService IftttService { get; set; }

        public IftttActor()
        {
			Receive<TriggerAction>(msg => {
				Context.Handle(msg, item => IftttService.TriggerAction(item.UserLogin, item.ActionId));
				});
        }
    }
}