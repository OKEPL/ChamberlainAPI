using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.Helpers;
using Chamberlain.Database.Persistency.Model;
using StructureMap.Attributes;
using System.Collections.Generic;
using System.Linq;
using Chamberlain.Common.Content.Commands;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Contracts.Enums;
using Chamberlain.Database.Persistency.Model.Enums;
using Chamberlain.Database.Persistency.Model.Extensions;
using Microsoft.EntityFrameworkCore;
using PredefinedRulesManager.Interfaces;
using RulesEngine.Contracts.Helpers;
using Serilog;
using Action = Chamberlain.Database.Persistency.Model.Action;
using ValueWithLabel = Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule.ValueWithLabel;

namespace Chamberlain.AppServer.Api.Services
{
    using Contracts.Models.RequestModels.Rule;

    public class RuleService : IRuleService
    {
        [SetterProperty] public IPredefinedRulesManagerPlugin PredefinedRulesManagerPlugin { get; set; }

        #region Rules Methods
        public List<RuleInfoModel> GetRules(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).First(x => x.Username.Equals(userName));
                
                return customer.Rules.Select(RuleToRuleModel).ToList();
            }
        }

        public RuleInfoModel GetRule(string userName, long ruleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).First(x => x.Username.Equals(userName));
                
                return RuleToRuleModel(customer.Rules.First(rule => rule.Id == ruleId));
            }
        }

        public RuleInfoModel AddRule(string userName, RuleInfoModel ruleModel, IActorRef ruleEngineActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                
                var rule = context.Rules.Add(new Rule
                {
                    Name = ruleModel.RuleName,
                    CustomerId = customer.Id
                }).Entity;
                SetRuleModes(context, rule, ruleModel.Modes.Cast<RuleModeModel>().ToList(), customer.Id);
                SetRuleNotifiers(rule, ruleModel.Notifiers);
                context.SaveChanges();

                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
                return RuleToRuleModel(rule);
            }
        }

        public RuleInfoModel UpdateRule(string userName, RuleInfoModel ruleModel, IActorRef ruleEngineActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).First(x => x.Username.Equals(userName));
                
                var rule = customer.Rules.First(x => x.Id == ruleModel.RuleId);
                if (ruleModel.RuleName != null)
                    rule.Name = ruleModel.RuleName;

                SetRuleModes(context, rule, ruleModel.Modes.Cast<RuleModeModel>().ToList(), customer.Id);
                SetRuleNotifiers(rule, ruleModel.Notifiers);
                context.SaveChanges();
                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
                return RuleToRuleModel(rule);
            }
        }

        public void DeleteRule(string userName, long ruleId, IActorRef ruleEngineActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).First(x => x.Username.Equals(userName));
                
                var rule = customer.Rules.FirstOrDefault(x => x.Id == ruleId);
                if (rule == null)
                    return;

                PredefinedRulesManagerPlugin.AddRuleToIgnoredIfPredefined(context, rule);

                rule.RemoveSafely(context);

                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
            }
        }
        #endregion

        #region Rule Modes region
        public List<RuleModeInfoModel> GetRuleModes(string userName, long ruleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).First(x => x.Username.Equals(userName));
                
                return GetRuleModes(customer, ruleId);
            }
        }

        public void SetRuleModes(string userName, long ruleId, List<RuleModeModel> ruleModes, IActorRef ruleEngineActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).First(x => x.Username.Equals(userName));
                
                var rule = customer.Rules.First(x => x.Id == ruleId);
                SetRuleModes(context, rule, ruleModes, customer.Id);
                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
            }
        }
        #endregion

        #region Rule Notifiers Methods
        public List<RuleNotifierModel> GetRuleNotifiers(string userName, long ruleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).First(x => x.Username.Equals(userName));
                
                var rule = customer.Rules.First(x => x.Id == ruleId);
                return GetNotifiersModel(rule);
            }
        }

        public void UpdateRuleNotifiers(string userName, ListRuleNotifiersModel notifiersList, long ruleId, IActorRef ruleEngineActorRef)
        {

            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).First(x => x.Username.Equals(userName));
                
                var rule = customer.Rules.First(x => x.Id == ruleId);
                SetRuleNotifiers(rule, notifiersList.NotifiersList);
                context.SaveChanges();
                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
            }
        }

        #endregion

        #region Rule Triggers Methods

        public RuleBaseTriggerModel AddRuleTrigger(string userName, RuleTriggerModel ruleTriggerModel, long ruleId, IActorRef ruleEngineActorRef, IActorRef clockTriggerManagerActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                var triggerGroup = context.Rules.FirstOrDefault(x => x.Id == ruleId)?.TriggerGroups.FirstOrDefault(x => x.ExecutionOrder == ruleTriggerModel.ExecutionOrder);

                if (triggerGroup == null)
                {
                    triggerGroup = new TriggerGroup
                    {
                        MaxConfirmationInterval = ruleTriggerModel.MaxConfirmationInterval,
                        ExecutionOrder = ruleTriggerModel.ExecutionOrder,
                        RuleId = ruleId
                    };
                    context.TriggerGroups.Add(triggerGroup);
                }

                var trigger = new Trigger
                {
                    ItemId = ruleTriggerModel.ItemId,
                    TriggerType = ruleTriggerModel.TriggerType,
                    ItemValue = ruleTriggerModel.ItemValue,
                    GreaterThan = ruleTriggerModel.GreaterThan,
                    LowerThan = ruleTriggerModel.LowerThan,
                    MinimalLastTime = ruleTriggerModel.MinimalLastTime,
                    TriggerGroup = triggerGroup
                };

                context.Triggers.Add(trigger);
                context.SaveChanges();

                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
                if (ClockTriggerHelper.TriggerTypeNames.Contains(trigger.TriggerType))
                    clockTriggerManagerActorRef.Tell(new ClockTriggerAddedMessage(trigger.Id));

                return new RuleBaseTriggerModel
                {
                    TriggerId = trigger.Id
                };
            }
        }

        public List<RuleTriggerModel> GetRuleTriggers(string userName, long ruleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.TriggerGroups).ThenInclude(x => x.Triggers).First(x => x.Username.Equals(userName));

                return customer.Rules.First(x => x.Id == ruleId).TriggerGroups.SelectMany(x => x.Triggers).Select(GetRuleTriggerModel).ToList();
            }
        }

        public List<TriggerWithDetails> GetRuleTriggersWithDetails(string userName, long ruleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.TriggerGroups).ThenInclude(x => x.Triggers).ThenInclude(x => x.Item).ThenInclude(x => x.Thing).Include(x => x.Rules).ThenInclude(x => x.TriggerGroups).ThenInclude(x => x.Triggers).ThenInclude(x => x.Item).ThenInclude(x => x.KnownItem).First(x => x.Username.Equals(userName));

                var rule = customer.Rules.First(x => x.Id == ruleId);
                    
                return rule.TriggerGroups
                    .SelectMany(x => x.Triggers)
                    .Select(TriggerToTriggerModelWithDetails).ToList();
            }
        }

        public RuleTriggerModel GetRuleTrigger(string userName, long triggerId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.TriggerGroups).ThenInclude(x => x.Triggers).First(x => x.Username.Equals(userName));
                
                var trigger = customer.Rules.SelectMany(x => x.TriggerGroups).SelectMany(x => x.Triggers).First(x => x.Id == triggerId);
                return GetRuleTriggerModel(trigger);
            }
        }

        public void DeleteRuleTrigger(string userName, long triggerId, IActorRef ruleEngineActorRef, IActorRef clockTriggerManagerActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.TriggerGroups).First(x => x.Username.Equals(userName));
                
                var triggerGroup = customer.Rules.SelectMany(x => x.TriggerGroups).FirstOrDefault(x => x.Triggers.Any(y => y.Id == triggerId));
                if (triggerGroup == null) return;

                var trigger = triggerGroup.Triggers.First(x => x.Id == triggerId);
                
                trigger.RemoveSafely(context);

                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
                if (ClockTriggerHelper.TriggerTypeNames.Contains(trigger.TriggerType))
                    clockTriggerManagerActorRef.Tell(new ClockTriggerDeletedMessage(trigger.Id));
            }
        }

        public void UpdateRuleTrigger(string userName, RuleTriggerModel ruleTriggerModel, IActorRef ruleEngineActorRef, IActorRef clockTriggerManagerActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                var trigger = customer.Rules.SelectMany(x => x.TriggerGroups).SelectMany(x => x.Triggers).FirstOrDefault(x => x.Id == ruleTriggerModel.TriggerId); 
                if (trigger == null) return;

                if (trigger.TriggerGroup.ExecutionOrder != ruleTriggerModel.ExecutionOrder)
                { 
                    var triggerGroup = trigger.TriggerGroup.Rule.TriggerGroups.SingleOrDefault(x => x.ExecutionOrder == ruleTriggerModel.ExecutionOrder) 
                           ?? 
                           new TriggerGroup
                            {
                                MaxConfirmationInterval = ruleTriggerModel.MaxConfirmationInterval,
                                ExecutionOrder = ruleTriggerModel.ExecutionOrder,
                                RuleId = trigger.TriggerGroup.RuleId,
                            };

                trigger.TriggerGroup = triggerGroup;
                }

                var triggerGroupsToDelete = trigger.TriggerGroup.Rule.TriggerGroups.Where(x => !x.Triggers.Any());
                context.TriggerGroups.RemoveRange(triggerGroupsToDelete);

                trigger.TriggerGroup.MaxConfirmationInterval = ruleTriggerModel.MaxConfirmationInterval;
                trigger.ItemId = ruleTriggerModel.ItemId;
                trigger.TriggerType = ruleTriggerModel.TriggerType;
                trigger.ItemValue = ruleTriggerModel.ItemValue;
                trigger.GreaterThan = ruleTriggerModel.GreaterThan;
                trigger.LowerThan = ruleTriggerModel.LowerThan;
                trigger.MinimalLastTime = ruleTriggerModel.MinimalLastTime;

                context.SaveChanges();

                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
                if (ClockTriggerHelper.TriggerTypeNames.Contains(trigger.TriggerType))
                    clockTriggerManagerActorRef.Tell(new ClockTriggerUpdatedMessage(trigger.Id));
            }
        }

        #endregion

        #region Rule Action Methods

        public RuleBaseActionModel AddRuleAction(string userName, RuleActionModel actionModel, long ruleId, IActorRef ruleEngineActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                var action = new Action
                {
                    ItemId = actionModel.ItemId,
                    ActionType = actionModel.ActionType,
                    RuleId = ruleId,
                    ItemValue = actionModel.ItemValue,
                    ActivationDelay = actionModel.DelayInSeconds,
                    ExecutionOrder = actionModel.ExecutionOrder,
                    Increase = actionModel.Increase,
                    Decrease = actionModel.Decrease
                };

                context.Actions.Add(action);
                context.SaveChanges();

                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));

                return new RuleBaseActionModel
                {
                    ActionId = action.Id
                };
            }
        }

        public List<RuleActionModel> GetRuleActions(string userName, long ruleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.Actions).First(x => x.Username.Equals(userName));
                
                return customer.Rules.First(x => x.Id == ruleId).Actions.Select(GetRuleActionModel).ToList();
            }
        }

        public List<ActionWithDetails> GetRuleActionsWithDetails(string userName, long ruleId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.Actions).First(x => x.Username.Equals(userName));
                
                return customer.Rules.First(x => x.Id == ruleId).Actions.Select(ActionToActionModelWithDetails).ToList();
            }
        }

        public RuleActionModel GetRuleAction(string userName, long actionId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.Actions).First(x => x.Username.Equals(userName));
                
                var action = customer.Rules.SelectMany(x => x.Actions).First(x => x.Id == actionId);
                return GetRuleActionModel(action);
            }
        }

        public void DeleteRuleAction(string userName, long actionId, IActorRef ruleEngineActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.Actions).First(x => x.Username.Equals(userName));
                var action = customer.Rules.SelectMany(x => x.Actions).FirstOrDefault(x => x.Id == actionId);
                if (action == null) return;

                context.Actions.Remove(action);
                context.SaveChanges();

                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
            }
        }

        public void UpdateRuleAction(string userName, RuleActionModel ruleActionModel, IActorRef ruleEngineActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Rules).ThenInclude(x => x.Actions).First(x => x.Username.Equals(userName));
 
                
                var action = customer.Rules.SelectMany(x => x.Actions).FirstOrDefault(x => x.Id == ruleActionModel.ActionId);
                if (action == null) return;

                action.ActionType = ruleActionModel.ActionType;
                action.ItemId = ruleActionModel.ItemId;
                action.ExecutionOrder = ruleActionModel.ExecutionOrder;
                action.ActivationDelay = ruleActionModel.DelayInSeconds;
                action.ItemValue = ruleActionModel.ItemValue;
                action.Increase = ruleActionModel.Increase;
                action.Decrease = ruleActionModel.Decrease;
                context.SaveChanges();

                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
            }
        }

        #endregion

        #region Possible Triggers/Actions

        public List<TriggerType> GetAvailableTriggerTypes(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).ThenInclude(x => x.KnownItem).First(x => x.Username.Equals(userName));

                
                var triggerTypes = GetClockTriggerTypes();
                
                var deviceBasicInfos = GetAvailableDevices(customer.Things.Where(x => x.KnownDeviceId != null && x.State == ThingStates.Active && x.Items.Any(y => y.KnownItem?.IsTrigger ?? false)));
                
                triggerTypes.Add(new TriggerType
                {
                    Name = "Devices",
                    Options = deviceBasicInfos,
                    RuleTriggerType = ValueTriggerHelper.Type
                });
                
                return triggerTypes;
            }
        }

        public List<DeviceBasicInfo> GetAvailableActionDevices(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).ThenInclude(x => x.KnownItem).First(x => x.Username.Equals(userName));

                return GetAvailableDevices(customer.Things.Where(x => x.KnownDeviceId != null && x.State == ThingStates.Active && x.Items.Any(y => y.KnownItem?.IsAction ?? false)));
            }
        }

        public List<DeviceItem> GetTriggersForDevice(string userName, long thingId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).ThenInclude(x => x.KnownItem).First(x => x.Username.Equals(userName));
                
                return TriggersOfThing(customer.Things.First(x => x.Id == thingId));
            }
        }

        public List<DeviceItem> GetActionsForDevice(string userName, long thingId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).ThenInclude(x => x.KnownItem).First(x => x.Username.Equals(userName));
                
                return ActionsOfThing(customer.Things.First(x => x.Id == thingId));
            }
        }

        public DeviceItemDetails GetAvailableTriggerDetails(string userName, long itemId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).ThenInclude(x => x.KnownItem).First(x => x.Username.Equals(userName));
                
                var item = customer.Things.SelectMany(x => x.Items).First(x => x.Id == itemId);
                return ItemToDeviceItemDetails(item, false);
            }
        }

        public DeviceItemDetails GetAvailableActionDetails(string userName, long itemId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).ThenInclude(x => x.KnownItem).First(x => x.Username.Equals(userName));
                
                var item = customer.Things.SelectMany(x => x.Items).First(x => x.Id == itemId);
                return ItemToDeviceItemDetails(item, true);
            }
        }
        #endregion

        private static RuleTriggerModel GetRuleTriggerModel(Trigger trigger)
        {
            return new RuleTriggerModel
            {
                MaxConfirmationInterval = trigger.TriggerGroup.MaxConfirmationInterval,
                ExecutionOrder = trigger.TriggerGroup.ExecutionOrder,
                ItemId = trigger.ItemId,
                TriggerId = trigger.Id,
                GreaterThan = trigger.GreaterThan,
                LowerThan = trigger.LowerThan,
                ItemValue = trigger.ItemValue,
                MinimalLastTime = trigger.MinimalLastTime,
                TriggerType = trigger.TriggerType,
                ThingId = trigger.Item?.ThingId
            };
        }

        private static RuleActionModel GetRuleActionModel(Action action)
        {
            return new RuleActionModel
            {
                ActionType = action.ActionType,
                ItemId = action.ItemId,
                ActionId = action.Id,
                ThingId = action.Item.ThingId,
                DelayInSeconds = action.ActivationDelay,
                ExecutionOrder = action.ExecutionOrder,
                ItemValue = action.ItemValue,
                Increase = action.Increase,
                Decrease = action.Decrease
            };
        }

        private static List<TriggerType> GetClockTriggerTypes()
        {
            return new List<TriggerType>
            {
                new TriggerType{ Name = "Clock" , RuleTriggerType = ClockTriggerHelper.Type.ClockTrigger.ToString() },
                new TriggerType{ Name = "Sunrise", RuleTriggerType = ClockTriggerHelper.Type.Sunrise.ToString()},
                new TriggerType{ Name = "Sunset", RuleTriggerType = ClockTriggerHelper.Type.Sunset.ToString() }
            };
        }

        private static DeviceItemDetails ItemToDeviceItemDetails(Item item, bool getActionDetails)
        {
            var result = new DeviceItemDetails
            {
                ItemId = item.Id,
                ItemType = item.Type,
                Name = item.CustomName,
                Options = new DeviceOptions()
            };

            if (item.KnownItem == null)
                return result;

            var knownSettings = BaseDeviceSettings.GetKnownItemSettings(item.KnownItem);

            if (knownSettings == null)
                return result;

            AddNumericValueOptions(knownSettings, result, getActionDetails);
            AddValueWithLabelOptions(knownSettings, result);
            AddUserDependantValuesOptions(item, knownSettings, result);
            result.Options.WebControlType =  knownSettings.WebControlType.ToString();                       
            return result;
        }

        private static void AddNumericValueOptions(KnownItemSettings knownSettings, DeviceItemDetails result, bool getActionDetails)
        {
            if (knownSettings.NumericValueOption == null)
                return;

            var optionsList = GetDefaultNumericOptionsList(knownSettings);

            if (getActionDetails)
                AddActionOptions(knownSettings, optionsList);
            else
                AddTriggerOptions(knownSettings, optionsList);

            result.Options = new NumericValueOptions()
                {
                    ValueOptions = optionsList
                };
        }

        private static List<NumericValueOption> GetDefaultNumericOptionsList(KnownItemSettings knownSettings)
        {
            return new List<NumericValueOption>
            {
                NumericValueOptionFromSettings(knownSettings, "Equal to", "itemValue", knownSettings.NumericValueOption.MinValue)
            };
        }

        private static void AddActionOptions(KnownItemSettings knownSettings, ICollection<NumericValueOption> optionsList)
        {
            optionsList.Add(NumericValueOptionFromSettings(knownSettings, "Increase", "increase", 1));
            optionsList.Add(NumericValueOptionFromSettings(knownSettings, "Decrease", "decrease", 1));
        }

        private static void AddTriggerOptions(KnownItemSettings knownSettings, ICollection<NumericValueOption> optionsList)
        {
            optionsList.Add(NumericValueOptionFromSettings(knownSettings, "Greater than", "greaterThan", 1));
            optionsList.Add(NumericValueOptionFromSettings(knownSettings, "Lower than", "lowerThan", 1));
        }

        private static void AddUserDependantValuesOptions(Item item, KnownItemSettings knownSettings, DeviceItemDetails result)
        {
            if (knownSettings.UserDependantValuesOption == null)
                return;
            
            if (knownSettings.UserDependantValuesOption.Type == "FaceProfile")
                using (var context = new Entities())
                {
                    result.Options = new ValueWithLabelOptions
                    {
                        Values = context.FaceProfiles
                            .Where(x => x.CustomerId == item.Thing.CustomerId)
                            .Select(x => new ValueWithLabel {Name = x.Name, Value = x.Id.ToString()})
                            .ToList()
                    };
                }
            
        }

        private static void AddValueWithLabelOptions(KnownItemSettings knownSettings, DeviceItemDetails result)
        {
            if (knownSettings.ValueWithLabelOption == null)
                return;

            result.Options = new ValueWithLabelOptions
            {
                Values = knownSettings.ValueWithLabelOption.PossibleValues.Select(x => new ValueWithLabel
                {
                    Name = x.Label,
                    Value = x.Value,
                }).ToList()
            };
        }

        private static NumericValueOption NumericValueOptionFromSettings(KnownItemSettings knownSettings, string name, string key, double minValue)
        {
            return new NumericValueOption
            {
                Name = name,
                Key = key,
                Unit = knownSettings.NumericValueOption.Unit,
                Min = minValue,
                Max = knownSettings.NumericValueOption.MaxValue,
                Step = knownSettings.NumericValueOption.Step
            };
        }

        private static DeviceItem ItemToDeviceItem(Item item)
        {
            return new DeviceItem
            {
                ItemId = item.Id,
                Name = item.CustomName
            };
        }

        private static List<RuleNotifierModel> GetNotifiersModel(Rule rule)
        {
            var result = new List<RuleNotifierModel>
            {
                new RuleNotifierModel
                {
                    Name = NotifierType.Sms.ToString(),
                    IsActive = rule.Sms
                },
                new RuleNotifierModel
                {
                    Name = NotifierType.Ifttt.ToString(),
                    IsActive = rule.Ifttt
                },
                new RuleNotifierModel
                {
                    Name = NotifierType.Voip.ToString(),
                    IsActive = rule.Voip
                },
                new RuleNotifierModel
                {
                    Name = NotifierType.Email.ToString(),
                    IsActive = rule.Email
                },
                new RuleNotifierModel
                {
                    Name = NotifierType.Warnings.ToString(),
                    IsActive = rule.Warning
                }
            };

            return result;
        }

        private static void SetRuleNotifiers(Rule rule, List<RuleNotifierModel> notifiersList)
        {
            foreach (var notifier in notifiersList)
            {
                if (notifier.Name == NotifierType.Sms.ToString())
                {
                    rule.Sms = notifier.IsActive;
                }
                else if (notifier.Name == NotifierType.Ifttt.ToString())
                {
                    rule.Ifttt = notifier.IsActive;
                }
                else if (notifier.Name == NotifierType.Voip.ToString())
                {
                    rule.Voip = notifier.IsActive;
                }
                else if (notifier.Name == NotifierType.Email.ToString())
                {
                    rule.Email = notifier.IsActive;
                }
                else if (notifier.Name == NotifierType.Warnings.ToString())
                {
                    rule.Warning = notifier.IsActive;
                }
            }
        }

        private static List<RuleModeInfoModel> GetRuleModes(Customer customer, long ruleId)
        {
            return customer.Modes.OrderBy(x => x.Id).Select(x => new RuleModeInfoModel
            {
                ModeId = x.Id,
                Name = x.Name,
                Color = x.Color,
                IsActive = x.ModeRuleBindings.Any(y => y.RuleId == ruleId)
            }).ToList();
        }

        private static void SetRuleModes(Entities context, Rule rule, List<RuleModeModel> ruleModes, long customerId)
        {
            foreach (var modeId in ruleModes.Where(x => x.IsActive).Select(x => x.ModeId))
            {
                var mode = context.Modes.First(x => x.Id == modeId);
                if (mode.CustomerId == customerId && rule.ModeRuleBindings.All(x => x.ModeId != modeId))
                {
                    context.ModeRuleBindings.Add(new ModeRuleBinding
                    {
                        ModeId = modeId,
                        RuleId = rule.Id
                    });
                }
            }

            context.ModeRuleBindings.RemoveRange(rule.ModeRuleBindings.Where(y => ruleModes.Where(x => !x.IsActive).Select(x => x.ModeId).Contains(y.ModeId)));
        }

        private static List<DeviceItem> TriggersOfThing(Thing thing)
        {
            return thing.Items.Where(x => x.KnownItem?.IsTrigger ?? false).Select(ItemToDeviceItem).ToList();
        }

        private static List<DeviceItem> ActionsOfThing(Thing thing)
        {
            return thing.Items.Where(x => x.KnownItem?.IsAction ?? false).Select(ItemToDeviceItem).ToList();
        }

        private static ActionWithDetails ActionToActionModelWithDetails(Action action)
        {
            return new ActionWithDetails
            {
                AvailableActions = action.Item != null ? ActionsOfThing(action.Item.Thing) : null,
                Details = action.Item != null ? ItemToDeviceItemDetails(action.Item, true) : null,
                Action = GetRuleActionModel(action)
            };
        }

        private static TriggerWithDetails TriggerToTriggerModelWithDetails(Trigger trigger)
        {
            return new TriggerWithDetails
            {
                AvailableTriggers = trigger.Item != null ? TriggersOfThing(trigger.Item.Thing) : null,
                Details = trigger.Item != null ? ItemToDeviceItemDetails(trigger.Item, false) : null,
                Trigger = GetRuleTriggerModel(trigger)
            };
        }

        private static RuleInfoModel RuleToRuleModel(Rule rule)
        {
            return new RuleInfoModel
            {
                RuleId = rule.Id,
                RuleName = rule.Name,
                IsPredefined = rule.PredefinedRuleId != null,
                Notifiers = GetNotifiersModel(rule),
                Modes = GetRuleModes(rule.Customer, rule.Id)
            };
        }

        private static List<DeviceBasicInfo> GetAvailableDevices(IEnumerable<Thing> things)
        {
            return things.Select(x => new DeviceBasicInfo
            {
                Name = string.IsNullOrEmpty(x.GivenName) ? x.NativeName : x.GivenName,
                ThingId = x.Id
            }).ToList();
        }
    }
}