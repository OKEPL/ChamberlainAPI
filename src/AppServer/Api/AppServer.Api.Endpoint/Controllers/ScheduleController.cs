using System;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region
    
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Schedules;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Schedule;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Schedule;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Schedule controller class.
    /// </summary>
    [Route("schedule")]
    public class ScheduleController : ChamberlainBaseController
    {
        /// <summary>
        /// Creates new schedule for user.
        /// </summary>
        /// <param name="schedule">
        /// Model describing new schedule.
        /// </param>
        /// <returns>
        /// Returns 201 if created successfully.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddSchedule([FromBody] ScheduleModel schedule)
        {
            await SystemActors.ScheduleActor.Execute(new AddSchedule(User.Identity.Name, schedule.Name));
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Delete schedule of given ScheduleId.
        /// </summary>
        /// <param name="scheduleId">
        /// ScheduleId of schedule to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{scheduleId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteSchedule(long scheduleId)
        {
            await SystemActors.ScheduleActor.Execute(new DeleteSchedule(User.Identity.Name, scheduleId));
            return NoContent();
        }

        /// <summary>
        /// Returns model of active schedule for user.
        /// </summary>
        /// <returns>
        /// Model describing active schedule.
        /// </returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ScheduleModel), 200)]
        public async Task<IActionResult> GetActiveSchedule()
        {
            var result = await SystemActors.ScheduleActor.Execute<GetActiveSchedule, ScheduleModel>(new GetActiveSchedule(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all schedule modes for current user.
        /// </summary>
        /// <returns>
        /// List of weeks schedules.
        /// </returns>
        [HttpGet("modes")]
        [ProducesResponseType(typeof(List<ScheduleModel>), 200)]
        public async Task<IActionResult> GetScheduledModes()
        {
            var result = await SystemActors.ScheduleActor.Execute<GetScheduledModes, List<DaysScheduleModel>>(new GetScheduledModes(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all schedule modes for user.
        /// </summary>
        /// <returns>
        /// List of schedules.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ScheduleModel>), 200)]
        public async Task<IActionResult> GetSchedules()
        {
            var result = await SystemActors.ScheduleActor.Execute<GetSchedules, List<ScheduleModel>>(new GetSchedules(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Add ScheduleModeEntry.
        /// </summary>
        /// <param name="scheduledModeEntryModel">
        /// Scheduled Mode Entry model.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPost("mode/add")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AddScheduleEntity([FromBody] ScheduledModeEntryModel scheduledModeEntryModel)
        {
            await SystemActors.ScheduleActor.Execute(
                new AddScheduleEntity(
                    User.Identity.Name,
                    scheduledModeEntryModel.ScheduleId,
                    scheduledModeEntryModel.ModeId,
                    scheduledModeEntryModel.WeekDay,
                    TimeSpan.Parse(scheduledModeEntryModel.StartAt),
                    scheduledModeEntryModel.Duration));
            return NoContent();
        }

        /// <summary>
        /// Activates first schedule for user.
        /// </summary>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("active")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SetFirstScheduleActive()
        {
            await SystemActors.ScheduleActor.Execute(new SetFirstScheduleActive(User.Identity.Name));
            return NoContent();
        }

        /// <summary>
        /// Actives schedule given by ScheduleId.
        /// </summary>
        /// <param name="scheduleId">
        /// ScheduleId of schedule to activate.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("activate/{scheduleId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SetActiveSchedule(long scheduleId)
        {
            await SystemActors.ScheduleActor.Execute(new SetActiveSchedule(User.Identity.Name, scheduleId));
            return NoContent();
        }

        /// <summary>
        /// Deactivates active schedule for user.
        /// </summary>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("deactivate")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SetInactiveSchedules()
        {
            await SystemActors.ScheduleActor.Execute(new SetInactiveSchedules(User.Identity.Name));
            return NoContent();
        }
        
        /// <summary>
        /// Update schedule of ScheduleId from model with new name from model.
        /// </summary>
        /// <param name="schedule">
        /// Schedule model, only ScheduleId and Name is important.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateSchedule([FromBody] ScheduleModel schedule)
        {
            await SystemActors.ScheduleActor.Execute(new UpdateScheduleName(User.Identity.Name, schedule.ScheduleId, schedule.Name));
            return NoContent();
        }
    }
}