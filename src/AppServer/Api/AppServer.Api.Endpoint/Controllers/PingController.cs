namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Ping controller class.
    /// </summary>
    [AllowAnonymous]
    [Route("ping")]
    public class PingController : ChamberlainBaseController
    {
        /// <summary>
        /// Pinging Get request.
        /// </summary>
        /// <returns>
        /// Should return "Pong".
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        public string Get()
        {
            return "Pong";
        }

        /// <summary>
        /// Pinging Post request.
        /// </summary>
        /// <returns>
        /// Should return "Pong".
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        public string Post()
        {
            return "Pong";
        }
    }
}