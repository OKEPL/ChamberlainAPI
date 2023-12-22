using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chamberlain.AppServer.Api.Contracts.Commands.Rules;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;
    using global::AppServer.Api.Endpoint.Controllers;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Rule controllerclass.
    /// </summary>
    [Route("rules")]
    public class RuleController : ChamberlainBaseController
    {
        #region Rule Methods
        /// <summary>
        /// Gets rule of id for current user.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule to get.
        /// </param>
        /// <returns>
        /// Rule model describing wanted rule.
        /// </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RuleInfoModel), 200)]
        public async Task<IActionResult> GetRule(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRule, RuleInfoModel>(new GetRule(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all rules from current user.
        /// </summary>
        /// <returns>
        /// List of rules for current user.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<RuleInfoModel>), 200)]
        public async Task<IActionResult> GetRules()
        {
            var result = await SystemActors.RuleActor.Execute<GetRules, List<RuleInfoModel>>(new GetRules(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Adds rule for current user.
        /// </summary>
        /// <param name="ruleModel">
        /// Model describing new rule.
        /// </param>
        /// <returns>
        /// Returns 201 if created successfully and created rule.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(RuleInfoModel), 201)]
        public async Task<IActionResult> AddRule([FromBody] RuleInfoModel ruleModel)
        {
            var result = await SystemActors.RuleActor.Execute<AddRule, RuleInfoModel>(new AddRule(User.Identity.Name, ruleModel));
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Update rule of given ruleId with rest of the properties.
        /// </summary>
        /// <param name="ruleModel">
        /// Rule model describing new rule. RuleId have to match updated one.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(typeof(RuleInfoModel), 200)]
        public async Task<IActionResult> UpdateRule([FromBody] RuleInfoModel ruleModel)
        {
            var result = await SystemActors.RuleActor.Execute<UpdateRule, RuleInfoModel>(new UpdateRule(User.Identity.Name, ruleModel));
            return Ok(result);
        }

        /// <summary>
        /// Deletes rule of id for current user.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteRule(long id)
        {
            await SystemActors.RuleActor.Execute(new DeleteRule(User.Identity.Name, id));
            return NoContent();
        }
        #endregion

        #region Rule Modes Methods
        /// <summary>
        /// Gets modes for rule of id for current user.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule to get modes.
        /// </param>
        /// <returns>
        /// Rule modes model describing wanted rule.
        /// </returns>
        [HttpGet("{id}/modes")]
        [ProducesResponseType(typeof(List<RuleModeInfoModel>), 200)]
        public async Task<IActionResult> GetRuleModes(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRuleModes, List<RuleModeInfoModel>>(new GetRuleModes(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Set active/not active modes for rule of id for current user.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule to set modes.
        /// </param>
        /// <param name="ruleModeModel">
        /// Modes to set.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("{id}/modes")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SetRuleModes(long id, [FromBody] List<RuleModeModel> ruleModeModel)
        {
            await SystemActors.RuleActor.Execute(new SetRuleModes(User.Identity.Name, id, ruleModeModel));
            return NoContent();
        }

        #endregion

        #region Rule Notifiers Methods
        /// <summary>
        /// Gets notifiers of rule id for current user.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule to get.
        /// </param>
        /// <returns>
        /// List of notifier model describing all notifers for selected rule.
        /// </returns>
        [HttpGet("{id}/notifiers")]
        [ProducesResponseType(typeof(List<RuleNotifierModel>), 200)]
        public async Task<IActionResult> GetRuleNotifier(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRuleNotifiers, List<RuleNotifierModel>>(new GetRuleNotifiers(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Update rule notifiers of given ruleId with rest of the properties.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule with notifiers to get.
        /// </param>
        /// <param name="notifiersList">
        /// NotifiersList containing NotifiersList of RuleNotifierModel.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("{id}/notifiers")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateRuleNotifiers([FromBody] ListRuleNotifiersModel notifiersList, long id)
        {
            await SystemActors.RuleActor.Execute(new UpdateRuleNotifiers(User.Identity.Name, notifiersList, id));
            return NoContent();
        }
        #endregion

        #region Rule Triggers Methods

        /// <summary>
        /// Adds trigger for selected item.
        /// </summary>
        /// /// <param name="id">
        /// RuleId of rule to add trigger.
        /// </param>
        /// <param name="ruleTriggerModel">
        /// Model describing new trigger.
        /// </param>
        /// <returns>
        /// Returns 201 and TriggerId if created successfully.
        /// </returns>
        [HttpPost("{id}/trigger")]
        [ProducesResponseType(typeof(RuleBaseTriggerModel), 201)]
        public async Task<IActionResult> AddRuleTrigger([FromBody] RuleTriggerModel ruleTriggerModel, long id)
        {
            var result = await SystemActors.RuleActor.Execute<AddRuleTrigger, RuleBaseTriggerModel>(new AddRuleTrigger(User.Identity.Name, ruleTriggerModel, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets triggers of rule id for current user.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule's trigger to get.
        /// </param>
        /// <returns>
        /// List of triggers for selected rule.
        /// </returns>
        [HttpGet("{id}/trigger")]
        [ProducesResponseType(typeof(List<RuleTriggerModel>), 200)]
        public async Task<IActionResult> GetRuleTriggers(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRuleTriggers, List<RuleTriggerModel>>(new GetRuleTriggers(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets triggers of rule id for current user with their details.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule's trigger to get.
        /// </param>
        /// <returns>
        /// List of triggers with their details for selected rule.
        /// </returns>
        [HttpGet("{id}/triggerWithDetails")]
        [ProducesResponseType(typeof(List<TriggerWithDetails>), 200)]
        public async Task<IActionResult> GetRuleTriggersWithDetails(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRuleTriggersWithDetails, List<TriggerWithDetails>>(new GetRuleTriggersWithDetails(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets trigger of given id for current user.
        /// </summary>
        /// <param name="id">
        /// TriggerId of trigger to get.
        /// </param>
        /// <returns>
        /// Trigger model describing wanted trigger.
        /// </returns>
        [HttpGet("trigger/{id}")]
        [ProducesResponseType(typeof(RuleTriggerModel), 200)]
        public async Task<IActionResult> GetRuleTrigger(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRuleTrigger, RuleTriggerModel>(new GetRuleTrigger(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Deletes trigger of id for current user.
        /// </summary>
        /// <param name="id">
        /// TriggerId of trigger to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("trigger/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteRuleTrigger(long id)
        {
            await SystemActors.RuleActor.Execute(new DeleteRuleTrigger(User.Identity.Name, id));
            return NoContent();
        }

        /// <summary>
        /// Update trigger of given triggerId with rest of the properties.
        /// </summary>
        /// <param name="ruleTriggerModel">
        /// RuleTrigger model describing new values to selected trigger.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("trigger")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateRuleTrigger([FromBody] RuleTriggerModel ruleTriggerModel)
        {
            await SystemActors.RuleActor.Execute(new UpdateRuleTrigger(User.Identity.Name, ruleTriggerModel));
            return NoContent();
        }

        #endregion

        #region Rule Action Methods

        /// <summary>
        /// Adds action for selected rule.
        /// </summary>
        /// /// <param name="id">
        /// RuleId of rule to add action.
        /// </param>
        /// <param name="ruleActionModel">
        /// Model describing new action.
        /// </param>
        /// <returns>
        /// Returns 201 and ActionId if created successfully.
        /// </returns>
        [HttpPost("{id}/action")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddRuleAction([FromBody] RuleActionModel ruleActionModel, long id)
        {
            var result = await SystemActors.RuleActor.Execute<AddRuleAction, RuleBaseActionModel>(new AddRuleAction(User.Identity.Name, ruleActionModel, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets actions of rule id for current user.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule's actions to get.
        /// </param>
        /// <returns>
        /// List of actions for selected rule.
        /// </returns>
        [HttpGet("{id}/action")]
        [ProducesResponseType(typeof(List<RuleActionModel>), 200)]
        public async Task<IActionResult> GetRuleActions(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRuleActions, List<RuleActionModel>>(new GetRuleActions(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets actions of rule id for current user with their details.
        /// </summary>
        /// <param name="id">
        /// RuleId of rule's actions to get.
        /// </param>
        /// <returns>
        /// List of actions with their details for selected rule.
        /// </returns>
        [HttpGet("{id}/actionWithDetails")]
        [ProducesResponseType(typeof(List<ActionWithDetails>), 200)]
        public async Task<IActionResult> GetRuleActionsWithDetails(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRuleActionsWithDetails, List<ActionWithDetails>>(new GetRuleActionsWithDetails(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets action of given id for current user.
        /// </summary>
        /// <param name="id">
        /// ActionId of action to get.
        /// </param>
        /// <returns>
        /// Action model describing wanted action.
        /// </returns>
        [HttpGet("action/{id}")]
        [ProducesResponseType(typeof(RuleActionModel), 200)]
        public async Task<IActionResult> GetRuleAction(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetRuleAction, RuleActionModel>(new GetRuleAction(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Deletes action of id for current user.
        /// </summary>
        /// <param name="id">
        /// ActionId of action to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("action/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteRuleAction(long id)
        {
            await SystemActors.RuleActor.Execute(new DeleteRuleAction(User.Identity.Name, id));
            return NoContent();
        }

        /// <summary>
        /// Update action of given actionId with rest of the properties.
        /// </summary>
        /// <param name="ruleActionModel">
        /// RuleAction model describing new values to selected action.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("action")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateRuleAction([FromBody] RuleActionModel ruleActionModel)
        {
            await SystemActors.RuleActor.Execute(new UpdateRuleAction(User.Identity.Name, ruleActionModel));
            return NoContent();
        }

        #endregion

        #region Possible Triggers/Actions

        /// <summary>
        /// Gets avaliable trigger types for current user.
        /// </summary>
        /// <returns>
        /// List of avaliable trigger types.
        /// </returns>
        [HttpGet("trigger/types")]
        [ProducesResponseType(typeof(List<TriggerType>), 200)]
        public async Task<IActionResult> GetAvailableTriggerTypes()
        {
            var result = await SystemActors.RuleActor.Execute<GetAvailableTriggerTypes, List<TriggerType>>(new GetAvailableTriggerTypes(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets avaliable action devices for current user.
        /// </summary>
        /// <returns>
        /// List of avaliable action Devices.
        /// </returns>
        [HttpGet("action/types")]
        [ProducesResponseType(typeof(List<DeviceBasicInfo>), 200)]
        public async Task<IActionResult> GetAvailableActionDevices()
        {
            var result = await SystemActors.RuleActor.Execute<GetAvailableActionDevices, List<DeviceBasicInfo>>(new GetAvailableActionDevices(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets avaliable triggers for particular device of current user.
        /// </summary>        
        /// <param name="id">
        /// ThingId of device.
        /// </param>
        /// <returns>
        /// List of avaliable triggers for device.
        /// </returns>
        [HttpGet("trigger/types/{id}")]
        [ProducesResponseType(typeof(List<DeviceItem>), 200)]
        public async Task<IActionResult> GetAvailableTriggers(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetAvailableTriggersForDevice, List<DeviceItem>>(new GetAvailableTriggersForDevice(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets avaliable actions for particular device of current user.
        /// </summary>
        /// <param name="id">
        /// ThingId of device.
        /// </param>
        /// <returns>
        /// List of avaliable actions for device.
        /// </returns>
        [HttpGet("action/types/{id}")]
        [ProducesResponseType(typeof(List<DeviceItem>), 200)]
        public async Task<IActionResult> GetAvailableActions(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetAvailableActionsForDevice, List<DeviceItem>>(new GetAvailableActionsForDevice(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets avalialble trigger details.
        /// </summary>        
        /// <param name="id">
        /// ItemId of trigger.
        /// </param>
        /// <returns>
        /// Details of avaliable trigger.
        /// </returns>
        [HttpGet("trigger/details/{id}")]
        [ProducesResponseType(typeof(DeviceItemDetails), 200)]
        public async Task<IActionResult> GetAvailableTriggerDetails(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetAvailableTriggerDetails, DeviceItemDetails>(new GetAvailableTriggerDetails(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets avalialble action details.
        /// </summary>        
        /// <param name="id">
        /// ItemId of action.
        /// </param>
        /// <returns>
        /// Details of avaliable action.
        /// </returns>
        [HttpGet("action/details/{id}")]
        [ProducesResponseType(typeof(DeviceItemDetails), 200)]
        public async Task<IActionResult> GetAvailableActionDetails(long id)
        {
            var result = await SystemActors.RuleActor.Execute<GetAvailableActionDetails, DeviceItemDetails>(new GetAvailableActionDetails(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        #endregion
    }
}