using Chamberlain.AppServer.Api.Contracts.Commands.Customers;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class CustomerSmsActor : Receiver
    {
        [SetterProperty]
        public ICustomerSmsService CustomerSmsService { get; set; }

        public CustomerSmsActor()
        {
			Receive<GetSms>(msg => {
				Context.Handle(msg, item => CustomerSmsService.GetSms(item.UserName));
				});
			Receive<AddSms>(msg => {
				Context.Handle(msg, item => CustomerSmsService.AddSms(item.UserName, item.Sms, item.Label, item.Alerts, item.Voip));
				});
			Receive<DeleteSms>(msg => {
				Context.Handle(msg, item => CustomerSmsService.DeleteSms(item.UserName, item.Sms));
				});
			Receive<UpdateSmses>(msg => {
				Context.Handle(msg, item => CustomerSmsService.UpdateSmses(item.UserName, item.SmsModelList));
				});
            Receive<UpdateSecurityPhones>(msg => {
                Context.Handle(msg, item => CustomerSmsService.UpdateSecurityPhones(item.UserName, item.SecurityPhoneModelList));
            });
        }
    }
}