using Chamberlain.AppServer.Api.Contracts.Commands.Customers;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class CustomerEmailActor : Receiver
    {
        [SetterProperty]
        public ICustomerEmailService CustomerEmailService { get; set; }

        public CustomerEmailActor()
        {
            Receive<AddEmail>(msg => {
                Context.Handle(msg, item => CustomerEmailService.AddEmail(item.UserName, item.Email, item.Alerts, item.Newsletter));
                });
            Receive<DeleteEmail>(msg => {
                Context.Handle(msg, item => CustomerEmailService.DeleteEmail(item.UserName, item.Email));
                });
            Receive<GetEmails>(msg => {
                Context.Handle(msg, item => CustomerEmailService.GetEmails(item.UserName));
                });
            Receive<UpdateEmails>(msg => {
                Context.Handle(msg, item => CustomerEmailService.UpdateEmails(item.UserName, item.EmailModelList));
                });
        }
    }
}