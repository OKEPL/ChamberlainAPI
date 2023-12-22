namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Home;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Event;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Status;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Home controllerclass.
    /// </summary>
    [Route("home")]
    public class HomeController : ChamberlainBaseController
    {
        /// <summary>
        /// Clear all events of current user.
        /// </summary>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete]
        [Route("clearEvents")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ClearEvents()
        {
            await SystemActors.HomeActor.Execute(new ClearEvents(User.Identity.Name));
            return NoContent();
        }

        /// <summary>
        /// Gets specific camera image coded as string.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of camera to get image from.
        /// </param>
        /// <returns>
        /// Image coded as string. First part should represent coding information.
        /// </returns>
        [HttpGet]
        [Route("cameraImageByThingId/{thingId:long}")]
        [ProducesResponseType(typeof(CameraImageModel), 200)]
        public async Task<IActionResult> GetCameraImageByThingId(long thingId)
        {
            var result = await SystemActors.HomeActor.Execute<GetCameraImageByThingId, CameraImageModel>(
                             new GetCameraImageByThingId(User.Identity.Name, thingId));
            return new ObjectResult(result);
        }
        
        /// <summary>
        /// Get all cameras from user.
        /// </summary>
        /// <returns>
        /// List of cameras available for user.
        /// </returns>
        [HttpGet]
        [Route("cameras")]
        [ProducesResponseType(typeof(List<CameraModel>), 200)]
        public async Task<IActionResult> GetCamerasWithImages()
        {
            var result = await SystemActors.HomeActor.Execute<GetCamerasWithImages, List<CameraModel>>(
                    new GetCamerasWithImages(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Returns number of first events in language.
        /// </summary>
        /// <param name="number">
        /// Number of first Events to taken.
        /// </param>
        /// <param name="language">
        /// Message language. Example: "en-us", "nl-NL", "pl-pl"
        /// </param>
        /// <returns>
        /// List of EventModels
        /// </returns>
        [HttpGet]
        [Route("events/{number:int}/{language}")]
        [ProducesResponseType(typeof(List<EventModel>), 200)]
        public async Task<IActionResult> GetEventsFromBegining(int number, string language)
        {
            var result = await SystemActors.HomeActor.Execute<GetEventsFromBegining, List<EventModel>>(
                             new GetEventsFromBegining(User.Identity.Name, number, language));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets number of last events starting from lastSeenEventId in language.
        /// </summary>
        /// <param name="number">
        /// Number of latest Events taken.
        /// </param>
        /// <param name="lastSeenEventId">
        /// EventId of last seen event.
        /// </param>
        /// <param name="language">
        /// Message language. Example: "en-us", "nl-NL", "pl-pl"
        /// </param>
        /// <returns>
        /// List of EventModels
        /// </returns>
        [HttpGet]
        [Route("events/{number:int}/{lastSeenEventId:int}/{language}")]
        [ProducesResponseType(typeof(List<EventModel>), 200)]
        public async Task<IActionResult> GetEventsFromId(int number, int lastSeenEventId, string language)
        {
            var result = await SystemActors.HomeActor.Execute<GetEventsFromName, List<EventModel>>(
                             new GetEventsFromName(User.Identity.Name, number, lastSeenEventId, language));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all events from repository which have higher Id than lastSeenEventId in given language.
        /// </summary>
        /// <param name="lastSeenEventId">
        /// EventId of last seen event.
        /// </param>
        /// <param name="language">
        /// Message language. Example: "en-us", "nl-NL", "pl-pl"
        /// </param>
        /// <returns>
        /// List of EventModels
        /// </returns>
        [HttpGet]
        [Route("events/new/{lastSeenEventId:int}/{language}")]
        [ProducesResponseType(typeof(List<EventModel>), 200)]
        public async Task<IActionResult> GetNewestEventsFromId(int lastSeenEventId, string language)
        {
            var result = await SystemActors.HomeActor.Execute<GetNewestEventsFromName, List<EventModel>>(
                             new GetNewestEventsFromName(User.Identity.Name, lastSeenEventId, language));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Returns status models in given language.
        /// </summary>
        /// <param name="language">
        /// Message language. Example: "en-us", "nl-NL", "pl-pl"
        /// </param>
        /// <returns>
        /// List of StatusModels
        /// </returns>
        [HttpGet]
        [Route("status/{language}")]
        [ProducesResponseType(typeof(StatusModel), 200)]
        public async Task<IActionResult> GetStatus(string language)
        {
            var result = await SystemActors.HomeActor.Execute<GetStatus, StatusModel>(new GetStatus(User.Identity.Name));
            return new ObjectResult(result);
        }
    }
}