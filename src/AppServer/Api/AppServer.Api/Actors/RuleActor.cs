using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Commands.Rules;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;
using System;

namespace Chamberlain.AppServer.Api.Actors
{
    public class RuleActor : Receiver
    {
        [SetterProperty]
        public IRuleService RuleService { get; set; }
        private IActorRef RulesEngineActorRef => Context.System.ActorSelection("user/ruleEngine").ResolveOne(TimeSpan.FromSeconds(10)).Result;
        private IActorRef ClockTriggerManagerActorRef => Context.System.ActorSelection("user/ClockTriggerManagerActor").ResolveOne(TimeSpan.FromSeconds(10)).Result;

        public RuleActor()
        {
			Receive<GetRuleModes>(msg => {
				Context.Handle(msg, item => RuleService.GetRuleModes(item.UserName, item.RuleId));
				});

			Receive<SetRuleModes>(msg => {
				Context.Handle(msg, item => RuleService.SetRuleModes(item.UserName, item.RuleId, item.RuleModes, RulesEngineActorRef));
				});

			Receive<GetRules>(msg => {
				Context.Handle(msg, item => RuleService.GetRules(item.UserName));
				});

			Receive<GetRule>(msg => {
				Context.Handle(msg, item => RuleService.GetRule(item.UserName, item.RuleId));
				});

			Receive<AddRule>(msg => {
				Context.Handle(msg, item => RuleService.AddRule(item.UserName, item.RuleModel, RulesEngineActorRef));
				});

			Receive<UpdateRule>(msg => {
				Context.Handle(msg, item => RuleService.UpdateRule(item.UserName, item.RuleModel, RulesEngineActorRef));
				});

			Receive<DeleteRule>(msg => {
				Context.Handle(msg, item => RuleService.DeleteRule(item.UserName, item.RuleId, RulesEngineActorRef));
				});

			Receive<GetRuleNotifiers>(msg => {
				Context.Handle(msg, item => RuleService.GetRuleNotifiers(item.UserName, item.RuleId));
				});

			Receive<UpdateRuleNotifiers>(msg => {
				Context.Handle(msg, item => RuleService.UpdateRuleNotifiers(item.UserName, item.NotifiersList, item.RuleId, RulesEngineActorRef));
				});

			Receive<AddRuleTrigger>(msg => {
				Context.Handle(msg, item => RuleService.AddRuleTrigger(item.UserName, item.RuleTriggerModel, item.RuleId, RulesEngineActorRef, ClockTriggerManagerActorRef));
				});

			Receive<GetRuleTriggers>(msg => {
				Context.Handle(msg, item => RuleService.GetRuleTriggers(item.UserName, item.RuleId));
				});

			Receive<GetRuleTriggersWithDetails>(msg => {
				Context.Handle(msg, item => RuleService.GetRuleTriggersWithDetails(item.UserName, item.RuleId));
				});

			Receive<GetRuleTrigger>(msg => {
				Context.Handle(msg, item => RuleService.GetRuleTrigger(item.UserName, item.TriggerId));
				});

			Receive<DeleteRuleTrigger>(msg => {
				Context.Handle(msg, item => RuleService.DeleteRuleTrigger(item.UserName, item.TriggerId, RulesEngineActorRef, ClockTriggerManagerActorRef));
				});

			Receive<UpdateRuleTrigger>(msg => {
				Context.Handle(msg, item => RuleService.UpdateRuleTrigger(item.UserName, item.RuleTriggerModel, RulesEngineActorRef, ClockTriggerManagerActorRef));
				});

			Receive<GetAvailableTriggerTypes>(msg => {
				Context.Handle(msg, item => RuleService.GetAvailableTriggerTypes(item.UserName));
				});

			Receive<GetAvailableActionDevices>(msg => {
				Context.Handle(msg, item => RuleService.GetAvailableActionDevices(item.UserName));
				});

			Receive<GetAvailableTriggersForDevice>(msg => {
				Context.Handle(msg, item => RuleService.GetTriggersForDevice(item.UserName, item.ThingId));
				});

			Receive<GetAvailableActionsForDevice>(msg => {
				Context.Handle(msg, item => RuleService.GetActionsForDevice(item.UserName, item.ThingId));
				});

			Receive<GetAvailableTriggerDetails>(msg => {
				Context.Handle(msg, item => RuleService.GetAvailableTriggerDetails(item.UserName, item.ItemId));
				});

			Receive<GetAvailableActionDetails>(msg => {
				Context.Handle(msg, item => RuleService.GetAvailableActionDetails(item.UserName, item.ItemId));
				});
				
			Receive<AddRuleAction>(msg => {
				Context.Handle(msg, item => RuleService.AddRuleAction(item.UserName, item.RuleActionModel, item.RuleId, RulesEngineActorRef));
				});
				
			Receive<GetRuleActions>(msg => {
				Context.Handle(msg, item => RuleService.GetRuleActions(item.UserName, item.RuleId));
				});

			Receive<GetRuleActionsWithDetails>(msg => {
				Context.Handle(msg, item => RuleService.GetRuleActionsWithDetails(item.UserName, item.RuleId));
				});

			Receive<GetRuleAction>(msg => {
				Context.Handle(msg, item => RuleService.GetRuleAction(item.UserName, item.ActionId));
				});

			Receive<DeleteRuleAction>(msg => {
				Context.Handle(msg, item => RuleService.DeleteRuleAction(item.UserName, item.ActionId, RulesEngineActorRef));
				});

			Receive<UpdateRuleAction>(msg => {
				Context.Handle(msg, item => RuleService.UpdateRuleAction(item.UserName, item.RuleActionModel, RulesEngineActorRef));
				});

        }
    }
}