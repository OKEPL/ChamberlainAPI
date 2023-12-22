using System;
using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Commands.Stb;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class StbActor : Receiver
    {
        [SetterProperty]
        public IStbService StbService { get; set; }
        private static IActorRef RulesEngineActorRef => Context.System.ActorSelection("user/ruleEngine").ResolveOne(TimeSpan.FromSeconds(10)).Result;

        public StbActor()
        {
            Receive<StbLogin>(msg => {
                Context.Handle(msg, item =>
                {
                    var ruleEngineActorRef = RulesEngineActorRef;
                    StbService.StbLogin(item.UserName, item.SolocooLogin, ruleEngineActorRef);
                });
            });
        }
    }
}