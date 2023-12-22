using Chamberlain.AppServer.Api.Contracts.Commands.Stb;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Stb;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Threading.Tasks;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Stb Controller class.
    /// </summary>
    [Route("stb")]
    public class StbController : ChamberlainBaseController
    {
        /// <summary>
        /// Login to Solocoo API
        /// </summary>
        /// <param name="model">
        /// Stb login model.
        /// </param>
        /// <returns>
        /// Returns 201 if logged in successfully.
        /// </returns>
        [HttpPost("login")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> StbLogin([FromBody] StbLoginModel model)
        {
            await SystemActors.StbActor.Execute(new StbLogin(User.Identity.Name, model.SolocooLogin));
            return Created(string.Empty, null);
        }
    }
}
