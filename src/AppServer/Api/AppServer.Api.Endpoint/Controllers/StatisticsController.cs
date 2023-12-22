namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Statistics;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Statistics;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Statistics controller class.
    /// </summary>
    [Route("statistics")]
    public class StatisticsController : ChamberlainBaseController
    {
        /// <summary>
        /// Get statistics of current user.
        /// </summary>
        /// <returns>
        /// User statistics in model.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(StatisticsModel), 200)]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await SystemActors.StatisticsActor.Execute<GetStatistics, StatisticsModel>(new GetStatistics(User.Identity.Name));
            return new ObjectResult(result);
        }
    }
}