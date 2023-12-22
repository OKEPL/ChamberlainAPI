using System.Collections.Generic;
using System.Threading.Tasks;
using Chamberlain.AppServer.Api.Contracts.Commands.Modes;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;
using Chamberlain.AppServer.Api.Endpoint.Helpers;
using Chamberlain.Common.Akka;
using Microsoft.AspNetCore.Mvc;
using AppServer.Api.Endpoint.Controllers;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Mode controller class.
    /// </summary>
    [Route("mode")]
    public class ModeController : ChamberlainBaseController
    {
        /// <summary>
        /// Adds mode for user.
        /// </summary>
        /// <param name="model">
        /// Model representing new mode.</param>
        /// <returns>
        /// Returns 201 if created successfully.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddMode([FromBody] ModePostModel model)
        {
            await SystemActors.ModeActor.Execute(new AddMode(User.Identity.Name, model));
            return Created(string.Empty, null);
        }
        
        /// <summary>
        /// Delete mode of given modeIf from user.
        /// </summary>
        /// <param name="modeId">
        /// ModeId of mode to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{modeId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteMode(long modeId)
        {
            await SystemActors.ModeActor.Execute(new DeleteMode(User.Identity.Name, modeId));
            return NoContent();
        }

        /// <summary>
        /// Gets mode of given modeId.
        /// </summary>
        /// <param name="modeId">
        /// ModeId of mode to get.
        /// </param>
        /// <returns>
        /// Asked mode model.
        /// </returns>
        [HttpGet("{modeId}")]
        [ProducesResponseType(typeof(ModeModel), 200)]
        public async Task<IActionResult> GetMode(long modeId)
        {
            var result = await SystemActors.ModeActor.Execute<GetMode, ModeModel>(new GetMode(User.Identity.Name, modeId));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all modes from user.
        /// </summary>
        /// <returns>
        /// List of user modes.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ModeModel>), 200)]
        public async Task<IActionResult> GetModes()
        {
            var result =
                await SystemActors.ModeActor.Execute<GetModes, List<ModeModel>>(new GetModes(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Update mode with new color.
        /// </summary>
        /// <param name="model">
        /// Model of new mode to be updated with new color.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("color")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateModeColor([FromBody] ModeModel model)
        {
            await SystemActors.ModeActor.Execute(new UpdateModeColor(User.Identity.Name, model.ModeId, model.Color));
            return NoContent();
        }
        
        /// <summary>
        /// Updates mode name.
        /// </summary>
        /// <param name="model">
        /// Model of new mode. Only modeId and name is taken.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateMode([FromBody] ModeModel model)
        {
            await SystemActors.ModeActor.Execute(new UpdateModeName(User.Identity.Name, model.ModeId, model.Name));
            return NoContent();
        }
    }
}