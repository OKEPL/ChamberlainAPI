namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Ifttts;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Ifttt controller class.
    /// </summary>
    [Route("ifttt")]
    public class IftttController : ChamberlainBaseController
    {
        /// <summary>
        /// Triggers action using ifttt.
        /// </summary>
        /// <param name="ruleId">
        /// RuleId to trigger
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("{ruleId:long}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> TriggerAction(long ruleId)
        {
            await SystemActors.IftttActor.Execute(new TriggerAction(User.Identity.Name, ruleId));
            await SystemActors.IftttActor.Execute(new TriggerAction(this.User.Identity.Name, ruleId));
            return this.NoContent();
        }
    }
}