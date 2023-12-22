using System.Collections.Generic;

namespace Chamberlain.AppServer.Api.Contracts.Commands.Rules
{
    #region

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule;

    #endregion

    #region Rule Methods

    public class GetRules : HasUserName
    {
        public GetRules(string userName)
            : base(userName)
        {
        }
    }

    public class GetRule : HasUserName
    {
        public GetRule(string userName, long ruleId)
            : base(userName)
        {
            RuleId = ruleId;
        }

        public long RuleId { get; }
    }

    public class AddRule : HasUserName
    {
        public AddRule(string userName, RuleInfoModel model)
            : base(userName)
        {
            RuleModel = model;
        }

        public RuleInfoModel RuleModel { get; }
    }

    public class UpdateRule : HasUserName
    {
        public UpdateRule(string userName, RuleInfoModel model)
            : base(userName)
        {
            RuleModel = model;
        }

        public RuleInfoModel RuleModel { get; }
    }

    public class DeleteRule : HasUserName
    {
        public DeleteRule(string userName, long ruleId)
            : base(userName)
        {
            RuleId = ruleId;
        }

        public long RuleId { get; }
    }

    #endregion

    #region Rule Modes Methods

    public class GetRuleModes : HasUserName
    {
        public GetRuleModes(string userName, long ruleId) 
            : base(userName)
        {
            RuleId = ruleId;
        }

        public long RuleId { get; }
    }

    public class SetRuleModes : HasUserName
    {
        public SetRuleModes(string userName, long ruleId, List<RuleModeModel> ruleModes) : base(userName)
        {
            RuleId = ruleId;
            RuleModes = ruleModes;
        }

        public long RuleId { get; set; }

        public List<RuleModeModel> RuleModes { get; set; }
    }

    #endregion

    #region Rule Notifiers Methods

    public class GetRuleNotifiers : HasUserName
    {
        public GetRuleNotifiers(string userName, long ruleId)
            : base(userName)
        {
            RuleId = ruleId;
        }

        public long RuleId { get; }
    }

    public class UpdateRuleNotifiers : HasUserName
    {
        public UpdateRuleNotifiers(string userName, ListRuleNotifiersModel notifiersList, long ruleId)
            : base(userName)
        {
            NotifiersList = notifiersList;
            RuleId = ruleId;
        }

        public long RuleId { get; }

        public ListRuleNotifiersModel NotifiersList { get; }
    }

    #endregion

    #region Rule Triggers Methods

    public class AddRuleTrigger : HasUserName
    {
        public AddRuleTrigger(string userName, RuleTriggerModel ruleTriggerModel, long ruleId)
            : base(userName)
        {
            RuleTriggerModel = ruleTriggerModel;
            RuleId = ruleId;
        }
        
        public RuleTriggerModel RuleTriggerModel { get; }

        public long RuleId { get; }
    }

    public class GetRuleTriggers : HasUserName
    {
        public GetRuleTriggers(string userName, long ruleId)
            : base(userName)
        {
            RuleId = ruleId;
        }

        public long RuleId { get; }
    }

    public class GetRuleTriggersWithDetails : HasUserName
    {
        public GetRuleTriggersWithDetails(string userName, long ruleId)
            : base(userName)
        {
            RuleId = ruleId;
        }

        public long RuleId { get; }
    }

    public class GetRuleTrigger : HasUserName
    {
        public GetRuleTrigger(string userName, long triggerId)
            : base(userName)
        {
            TriggerId = triggerId;
        }

        public long TriggerId { get; }
    }

    public class DeleteRuleTrigger : HasUserName
    {
        public DeleteRuleTrigger(string userName, long triggerId)
            : base(userName)
        {
            TriggerId = triggerId;
        }

        public long TriggerId { get; }
    }

    public class UpdateRuleTrigger : HasUserName
    {
        public UpdateRuleTrigger(string userName, RuleTriggerModel ruleTriggerModel)
            : base(userName)
        {
            RuleTriggerModel = ruleTriggerModel;
        }

        public RuleTriggerModel RuleTriggerModel { get; }
    }

    #endregion

    #region Rule Action Methods

    public class AddRuleAction : HasUserName
    {
        public AddRuleAction(string userName, RuleActionModel ruleActionModel, long id)
            : base(userName)
        {
            RuleActionModel = ruleActionModel;
            RuleId = id;
        }

        public RuleActionModel RuleActionModel { get; }

        public long RuleId { get; }
    }

    public class GetRuleActions : HasUserName
    {
        public GetRuleActions(string userName, long ruleId)
            : base(userName)
        {
            RuleId = ruleId;
        }

        public long RuleId { get; }
    }

    public class GetRuleActionsWithDetails : HasUserName
    {
        public GetRuleActionsWithDetails(string userName, long ruleId)
            : base(userName)
        {
            RuleId = ruleId;
        }

        public long RuleId { get; }
    }


    public class GetRuleAction : HasUserName
    {
        public GetRuleAction(string userName, long actionId)
            : base(userName)
        {
            ActionId = actionId;
        }

        public long ActionId { get; }
    }

    public class DeleteRuleAction : HasUserName
    {
        public DeleteRuleAction(string userName, long actionId)
            : base(userName)
        {
            ActionId = actionId;
        }

        public long ActionId { get; }
    }

    public class UpdateRuleAction : HasUserName
    {
        public UpdateRuleAction(string userName, RuleActionModel ruleActionModel)
            : base(userName)
        {
            RuleActionModel = ruleActionModel;
        }

        public RuleActionModel RuleActionModel { get; }
    }

    #endregion

    #region Possible Triggers/Actions

    public class GetAvailableTriggerTypes : HasUserName
    {
        public GetAvailableTriggerTypes(string userName)
            : base(userName) { }
    }

    public class GetAvailableActionDevices : HasUserName
    {
        public GetAvailableActionDevices(string userName)
            : base(userName) { }
    }

    public class GetAvailableTriggersForDevice : HasUserName
    {
        public GetAvailableTriggersForDevice(string userName, long thingId)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long ThingId { get; }
    }

    public class GetAvailableActionsForDevice : HasUserName
    {
        public GetAvailableActionsForDevice(string userName, long thingId)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long ThingId { get; }
    }

    public class GetAvailableTriggerDetails : HasUserName
    {
        public GetAvailableTriggerDetails(string userName, long itemId)
            : base(userName)
        {
            ItemId = itemId;
        }

        public long ItemId { get; }
    }

    public class GetAvailableActionDetails : HasUserName
    {
        public GetAvailableActionDetails(string userName, long itemId)
            : base(userName)
        {
            ItemId = itemId;
        }

        public long ItemId { get; }
    }

    #endregion
}