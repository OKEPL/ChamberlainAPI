using Chamberlain.AppServer.Api.Contracts.Commands.Customers;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class CustomerIftttActor : Receiver
    {
        [SetterProperty]
        public ICustomerIfttService CustomerIfttService { get; set; }

        public CustomerIftttActor()
        {
			Receive<GetIfttt>(msg => {
				Context.Handle(msg, item => CustomerIfttService.GetIfttt(item.UserName));
				});
			Receive<AddIfttt>(msg => {
				Context.Handle(msg,
					item => CustomerIfttService.AddIfttt(item.UserName, item.Ifttt, item.Label, item.Alerts));
				});
			Receive<DeleteIfttt>(msg => {
				Context.Handle(msg, item => CustomerIfttService.DeleteIfttt(item.UserName, item.Ifttt));
				});
			Receive<UpdateIfttts >(msg => {
				Context.Handle(msg, item => CustomerIfttService.UpdateIfttts(item.UserName, item.IftttModelList));
				});
        }
    }
}