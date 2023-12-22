using System;
using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Commands.Customers;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class CustomerActor : Receiver
    {
        private readonly ActorSelection _ruleEngineActorRef = Context.System.ActorSelection("user/ruleEngine");

        [SetterProperty]
        public ICustomerService CustomerService { get; set; }

        public CustomerActor()
        {
            Receive<AddUser>(msg => {

                var actorRef = _ruleEngineActorRef.ResolveOne(TimeSpan.FromSeconds(10)).Result;
                Context.Handle(msg,
                    item => CustomerService.AddUser(item.Name, item.Email, item.Password, actorRef));
            });

            Receive<ChangeTimezone>(msg => {
                Context.Handle(msg, item => CustomerService.ChangeTimezone(item.UserName, item.Timezone));
            });
            Receive<GetUser>(msg => {
                Context.Handle(msg, item => CustomerService.GetUser(item.UserName));
            });
            Receive<GetUserSubscription>(msg => {
                Context.Handle(msg, item => CustomerService.GetUserSubscription(item.UserName));
            });
            Receive<ChangeUserSubscription>(msg => {
                Context.Handle(msg, item => CustomerService.ChangeUserSubscription(item.UserName, item.FeatureId));
            });
            Receive<ChangeUserMode>(msg => {

                var actorRef = _ruleEngineActorRef.ResolveOne(TimeSpan.FromSeconds(10)).Result;
                Context.Handle(msg, item => CustomerService.ChangeUserMode(item.UserName, item.ModeId, actorRef));
            });

            Receive<GetAccountData>(msg => {
                Context.Handle(msg, item => CustomerService.GetAccountData(item.UserName));
            });
            Receive<UpdateNotifications>(msg => {
                Context.Handle(msg, item => CustomerService.UpdateNotification(item.UserName, item.Data));
            });
            Receive<UpdateRecordingBracketsTime>(msg => {
                Context.Handle(msg,
                    item => CustomerService.UpdateRecordingBracketsTime(item.UserName, item.PreRecTime, item.PostRecTime));
            });
            Receive<DiscardNestConnection>(msg => {
                Context.Handle(msg, item => CustomerService.DiscardNestConnection(item.UserName));
            });
            Receive<GetNestToken>(msg => {
                Context.Handle(msg, item => CustomerService.GetNestTokenAndRedirectBack(msg.State, msg.Code));
            });
            Receive<GetAddress>(msg => {
                Context.Handle(msg, item => CustomerService.GetAddress(item.UserName));
            });
            Receive<AssignAddress>(msg => {
                Context.Handle(msg, item => CustomerService.AssignAddress(item.UserName, item.Data));
            });
            Receive<UpdateAddress>(msg => {
                Context.Handle(msg, item => CustomerService.UpdateAddress(item.UserName, item.Data));
            });
            Receive<CreateNestAuthenticationSession>(msg => {
                Context.Handle(msg, item => CustomerService.CreateNestAuthenticationSession(item.UserName, item.RedirectTo));
            });
            Receive<GetTimezones>(msg => {
                Context.Handle(msg, item => CustomerService.GetTimezones());
            });
        }
    }
}