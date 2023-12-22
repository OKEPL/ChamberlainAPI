namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Recordings;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Recordings;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Recording controller class.
    /// </summary>
    [Route("recording")]
    public class RecordingController : ChamberlainBaseController
    {
        /// <summary>
        /// Gets specific regording using given id.
        /// </summary>
        /// <param name="id">
        /// Id of recording want to get.
        /// </param>
        /// <returns>
        /// Recording model
        /// </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RecordingModel), 200)]
        public async Task<IActionResult> GetRecording(long id)
        {
            var result = await SystemActors.RecordingActor.Execute<GetRecording, RecordingModel>(new GetRecording(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets recordings for selected date.
        /// </summary>
        /// <param name="date">
        /// Get recordings for given date day.
        /// </param>
        /// <returns>
        /// List of Recording models.
        /// </returns>
        [HttpGet]
        [Route("byDate/{date}")]
        [ProducesResponseType(typeof(List<RecordingModel>), 200)]
        public async Task<IActionResult> GetRecordingsByDate(string date)
        {
            var result = await SystemActors.RecordingActor.Execute<GetRecordingsByDate, List<RecordingModel>>(new GetRecordingsByDate(User.Identity.Name, date));
            return new ObjectResult(result);
        }
        
        /// <summary>
        /// Gets recordings start, end and excluded dates for user.
        /// </summary>
        /// <returns>
        /// Model of StartDate, EndDate and all ExcludedDates</returns>
        [HttpGet]
        [Route("list/excludedDates")]
        [ProducesResponseType(typeof(RecordingExcludedDatesModel), 200)]
        public async Task<IActionResult> GetRecordingsBindingDates()
        {
            var result = await SystemActors.RecordingActor.Execute<GetRecordingExcludedDates, RecordingExcludedDatesModel>(new GetRecordingExcludedDates(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets recordings dates for user.
        /// </summary>
        /// <returns>
        /// List of dateTimes
        /// </returns>
        [HttpGet]
        [Route("list/dates")]
        [ProducesResponseType(typeof(List<DateTime>), 200)]
        public async Task<IActionResult> GetRecordingsDates()
        {
            var result = await SystemActors.RecordingActor.Execute<GetRecordingDates, List<DateTime>>(new GetRecordingDates(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Remove video of id from user.
        /// </summary>
        /// <param name="id">
        /// Recording id of video to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> RemoveVideo(long id)
        {
            await SystemActors.RecordingActor.Execute(new DeleteRecording(User.Identity.Name, id));
            return NoContent();
        }

        /// <summary>
        /// Remove video list of id from user.
        /// </summary>
        /// <param name="id">
        /// Id of list to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete]
        [Route("list/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> RemoveVideo(string id)
        {
            await SystemActors.RecordingActor.Execute(new DeleteRecordingList(User.Identity.Name, id));
            return NoContent();
        }

        /// <summary>
        /// Mark recording as seen.
        /// </summary>
        /// <param name="id">
        /// Id of recording to be marked as seen.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut]
        [Route("seen/{id:long}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> VideoSeen(long id)
        {
            await SystemActors.RecordingActor.Execute(new MarkRecordingAsSeen(User.Identity.Name, id));
            return NoContent();
        }
    }
}