using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Commands.Modes;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;
using System;

namespace Chamberlain.AppServer.Api.Actors
{
    public class ModeActor : Receiver
    {
        [SetterProperty]
        public IModeService ModeService { get; set; }
        private readonly ActorSelection RuleEngineActorRef = Context.System.ActorSelection("user/ruleEngine");

        public ModeActor()
        {
			Receive<GetModes>(msg => {
				Context.Handle(msg, item => ModeService.GetModes(item.UserName));
			});

			Receive<GetMode>(msg => {
				Context.Handle(msg, item => ModeService.GetMode(item.UserName, item.ModeId));
			});

			Receive<AddMode>(msg => {
				Context.Handle(msg, item =>
				{
					var actorRef = RuleEngineActorRef.ResolveOne(TimeSpan.FromSeconds(10)).Result;
					ModeService.AddMode(item.UserName, item.Model, actorRef);
				});
			});

			Receive<UpdateModeName>(msg => {
				Context.Handle(msg, item => ModeService.UpdateMode(item.UserName, item.ModeId, item.Name));
			});

			Receive<UpdateModeColor>(msg => {
				Context.Handle(msg, item => ModeService.UpdateModeColor(item.UserName, item.ModeId, item.Color));
			});

			Receive<DeleteMode>(msg => {
				Context.Handle(msg, item =>
				{
					var actorRef = RuleEngineActorRef.ResolveOne(TimeSpan.FromSeconds(10)).Result;
					ModeService.DeleteMode(item.UserName, item.ModeId, actorRef);
				});
			});

			Receive<UpdateMode>(msg => {
				Context.Handle(msg, item =>
				{
					var actorRef = RuleEngineActorRef.ResolveOne(TimeSpan.FromSeconds(10)).Result;
					ModeService.Update(item.UserName, item.Model, actorRef);
				});
			});
        }
    }
}
