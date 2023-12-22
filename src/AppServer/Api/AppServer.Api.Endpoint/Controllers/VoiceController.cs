using System;
using System.Data;
using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Voice;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Voice;
using Chamberlain.Common.Content.VoiceCommands;
using Chamberlain.Common.Contracts.Enums;
using VoiceApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Threading.Tasks;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Voice controller class.
    /// </summary>
    [Route("voice")]
    public class VoiceController : ChamberlainBaseController
    {
        /// <summary>
        /// Interpretes voice command
        /// </summary>
        /// <param name="request">Text of the command.</param>
        /// <returns>
        /// 200 if command was understood
        /// </returns>
        [HttpPost("interpret")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Interpret([FromBody] VoiceInterpretationRequestModel request)
        {
            var voiceRequest = new VoiceInterpretationRequest(await SystemActors.VoiceActor.ResolveOne(TimeSpan.FromSeconds(10)), VoiceCommandContext.Mobile, request.Text, User.Identity.Name);

            var result = SystemActors.VoiceActor.Ask<InterpretationCommand.InterpretationResponse>(new InterpretationCommand.SendRequest(voiceRequest)).Result;
            var resultModel = new VoiceResultModel {DidSucceed = result.Success, Message = result.ResponseText};

            if (result.Success)
                return Ok(resultModel);

            return BadRequest(resultModel);
        }
    }
}