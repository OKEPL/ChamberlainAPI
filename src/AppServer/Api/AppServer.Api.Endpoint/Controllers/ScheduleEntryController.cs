namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Schedules;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Schedule;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Schedule entry controller class.
    /// </summary>
    [Route("schedule/entry")]
    public class ScheduleEntryController : ChamberlainBaseController
    {
        /// <summary>
        /// Add new schedule given in model.
        /// </summary>
        /// <param name="scheduleMode">
        /// Schedule mode model to add.
        /// </param>
        /// <returns>
        /// Returns 201 if created successfully.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddScheduleEntry([FromBody] ScheduledModeModel scheduleMode)
        {
            await SystemActors.ScheduleActor.Execute(
                new AddScheduleEntry(
                    this.User.Identity.Name,
                    scheduleMode.ScheduleId,
                    scheduleMode.ModeId,
                    scheduleMode.Weekday,
                    scheduleMode.StartTime,
                    scheduleMode.Duration));
            return this.Created(string.Empty, null);
        }

        /// <summary>
        /// Deletes schedule entry of given id.
        /// </summary>
        /// <param name="id">
        /// ScheduleId entry to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteScheduleEntry(long id)
        {
            await SystemActors.ScheduleActor.Execute(new DeleteScheduleEntry(this.User.Identity.Name, id));
            return NoContent();
        }

        /// <summary>
        /// Update ScheduleMode of ScheduleId to rest of it properties.
        /// </summary>
        /// <param name="scheduleMode">
        /// ScheduleId to be updated and new properties values.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateScheduleEntry([FromBody] ScheduledModeModel scheduleMode)
        {
            await SystemActors.ScheduleActor.Execute(
                new UpdateScheduleEntry(
                    User.Identity.Name,
                    scheduleMode.Id,
                    scheduleMode.ScheduleId,
                    scheduleMode.ModeId,
                    scheduleMode.Weekday,
                    scheduleMode.StartTime,
                    scheduleMode.Duration));

            return NoContent();
        }
    }
}