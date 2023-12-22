using Chamberlain.AppServer.Api.Contracts.Commands.Schedules;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class ScheduleActor : Receiver
    {
        [SetterProperty]
        public IScheduleService ScheduleService { get; set; }

        public ScheduleActor()
        {
			Receive<GetSchedules>(msg => {
				Context.Handle(msg, item => ScheduleService.GetSchedules(item.UserName));
				});
			Receive<GetScheduledModes>(msg => {
				Context.Handle(msg, item => ScheduleService.GetScheduledModes(item.UserName));
				});
			Receive<GetActiveSchedule>(msg => {
				Context.Handle(msg, item => ScheduleService.GetActiveSchedule(item.UserName));
				});
			Receive<SetActiveSchedule>(msg => {
				Context.Handle(msg, item => ScheduleService.SetActiveSchedule(item.UserName, item.ScheduleId));
				});
			Receive<SetFirstScheduleActive>(msg => {
				Context.Handle(msg, item => ScheduleService.SetActiveDefaultSchedule(item.UserName));
				});
			Receive<SetInactiveSchedules>(msg => {
				Context.Handle(msg, item => ScheduleService.SetInactiveSchedules(item.UserName));
				});
			Receive<AddSchedule>(msg => {
				Context.Handle(msg, item => ScheduleService.AddSchedule(item.UserName, item.Name));
				});
			Receive<UpdateScheduleName>(msg => {
				Context.Handle(msg, item => ScheduleService.UpdateScheduleName(item.UserName, item.ScheduleId, item.Name));
				});
			Receive<DeleteSchedule>(msg => {
				Context.Handle(msg, item => ScheduleService.DeleteSchedule(item.UserName, item.ScheduleId));
				});
			Receive<AddScheduleEntry>(msg => {
				Context.Handle(msg, item => ScheduleService.AddScheduleEntry(item.UserName, item.ScheduleId, item.ModeId, item.WeekDay, item.StartTime, item.Duration));
				});
			Receive<UpdateScheduleEntry>(msg => {
				Context.Handle(msg, item => ScheduleService.UpdateScheduleEntry(item.UserName, item.EntryId, item.ScheduleId, item.ModeId, item.WeekDay, item.StartTime, item.Duration));
				});
			Receive<DeleteScheduleEntry>(msg => {
				Context.Handle(msg, item => ScheduleService.DeleteScheduleEntry(item.UserName, item.EntryId));
				});
			Receive<AddScheduleEntity>(msg => {
				Context.Handle(msg, item => ScheduleService.ScheduleEntry(item.UserName, item.ScheduleId, item.ModeId, item.WeekDay, item.StartAt, item.Duration));
				});
        }
    }
}