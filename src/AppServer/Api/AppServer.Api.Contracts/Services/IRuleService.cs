using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule;

namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using System.Collections.Generic;
    using Models.RequestModels.Rule;
    using Akka.Actor;

    #endregion

    public interface IRuleService
    {
        List<RuleInfoModel> GetRules(string userName);
        RuleInfoModel GetRule(string userName, long ruleId);
        RuleInfoModel AddRule(string userName, RuleInfoModel ruleModel, IActorRef actorRef);
        RuleInfoModel UpdateRule(string userName, RuleInfoModel ruleModel, IActorRef actorRef);
        void DeleteRule(string userName, long ruleId, IActorRef actorRef);
        List<RuleModeInfoModel> GetRuleModes(string userName, long ruleId);
        void SetRuleModes(string userName, long ruleId, List<RuleModeModel> ruleModes, IActorRef actorRef);
        RuleBaseTriggerModel AddRuleTrigger(string userName, RuleTriggerModel ruleTriggerModel, long ruleId, IActorRef actorRef, IActorRef clockTriggerManagerActorRef);
        List<RuleTriggerModel> GetRuleTriggers(string userName, long ruleId);
        List<TriggerWithDetails> GetRuleTriggersWithDetails(string userName, long ruleId);
        RuleTriggerModel GetRuleTrigger(string userName, long triggerId);
        void DeleteRuleTrigger(string userName, long triggerId, IActorRef actorRef, IActorRef clockTriggerManagerActorRef);
        void UpdateRuleTrigger(string userName, RuleTriggerModel ruleTriggerModel, IActorRef actorRef, IActorRef clockTriggerManagerActorRef);
        List<RuleNotifierModel> GetRuleNotifiers(string itemUserName, long ruleId);
        void UpdateRuleNotifiers(string userName, ListRuleNotifiersModel notifiersList, long ruleId, IActorRef actorRef);
        RuleBaseActionModel AddRuleAction(string userName, RuleActionModel actionModel, long ruleId, IActorRef actorRef);
        List<RuleActionModel> GetRuleActions(string userName, long ruleId);
        List<ActionWithDetails> GetRuleActionsWithDetails(string userName, long ruleId);
        RuleActionModel GetRuleAction(string userName, long actionId);
        void DeleteRuleAction(string userName, long actionId, IActorRef actorRef);
        void UpdateRuleAction(string userName, RuleActionModel ruleActionModel, IActorRef actorRef);
        List<TriggerType> GetAvailableTriggerTypes(string userName);
        List<DeviceBasicInfo> GetAvailableActionDevices(string userName);
        List<DeviceItem> GetTriggersForDevice(string userName, long thingId);
        List<DeviceItem> GetActionsForDevice(string userName, long thingId);
        DeviceItemDetails GetAvailableTriggerDetails(string userName, long itemId);
        DeviceItemDetails GetAvailableActionDetails(string userName, long itemId);
    }
}
