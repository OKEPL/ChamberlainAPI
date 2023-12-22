using System.Threading.Tasks;
using Chamberlain.AppServer.Api.Contracts.Commands.Tests;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Test;
using Chamberlain.AppServer.Api.Endpoint.Helpers;
using Chamberlain.AppServer.Api.Endpoint.Models;
using Chamberlain.Common.Akka;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Api.Endpoint.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Test Controller class
    /// </summary>
    [Route("tests")]
    public class TestController : ChamberlainBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Main constructor.
        /// </summary>
        public TestController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Simulates fall detection trigger activation
        /// </summary>
        /// <returns>
        /// Returns 204 if succeeded or didn't find user.
        /// </returns>
        [HttpPut("simulateFallDetection")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SimulateFallDetection()
        {
            await SystemActors.TestActor.Execute(new SimulateFallDetection(User.Identity.Name));
            return NoContent();
        }
        
        /// <summary>
        /// Simulates motion detection trigger activation
        /// </summary>
        /// <returns>
        /// Returns 204 if succeeded or didn't find user.
        /// </returns>
        [HttpPut("simulateMotionDetection")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SimulateMotionDetection()
        {
            await SystemActors.TestActor.Execute(new SimulateMotionDetection(User.Identity.Name));
            return NoContent();
        }

        /// <summary>
        /// Simulates trigger activation
        /// </summary>
        /// <param name="testTriggerModel">
        /// Model for activating trigger. Takes trigger type
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded or didn't find user.
        /// </returns>
        [HttpPut("simulateTrigger")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SimulateTrigger([FromBody] TestTriggerModel testTriggerModel)
        {
            await SystemActors.TestActor.Execute(new SimulateTrigger(User.Identity.Name, testTriggerModel));
            return NoContent();
        }
    }
}