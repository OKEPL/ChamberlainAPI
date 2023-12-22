using Chamberlain.AppServer.Api.Contracts.Commands.Customers;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class CustomerPushActor : Receiver
    {
        [SetterProperty]
        public ICustomerPushService CustomerPushService { get; set; }

        public CustomerPushActor()
        {
			Receive<GetFirebasePush>(msg => {
				Context.Handle(msg, item => CustomerPushService.GetFirebasePush(item.UserName));
				});

			Receive<AddFirebasePush>(msg => {
				Context.Handle(msg,
					item => CustomerPushService.AddFirebasePush(item.UserName, item.Firebase, item.Label,
						item.Alerts));
				});

			Receive<DeleteFirebasePush>(msg => {
				Context.Handle(msg, item => CustomerPushService.DeleteFirebasePush(item.UserName, item.Firebase));
				});
        }
    }
}