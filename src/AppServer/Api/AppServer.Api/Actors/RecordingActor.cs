using Chamberlain.AppServer.Api.Contracts.Commands.Recordings;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class RecordingActor : Receiver
    {
        [SetterProperty]
        public IRecordingService RecordingService { get; set; }

        public RecordingActor()
        {

			Receive<GetRecordingsByDate>(msg => {
				Context.Handle(msg, item => RecordingService.GetRecordingsByDate(item.UserName, item.Date));
				});
			Receive<GetRecording>(msg => {
				Context.Handle(msg, item => RecordingService.GetRecording(item.UserName, item.RecordingId));
				});
			Receive<MarkRecordingAsSeen>(msg => {
				Context.Handle(msg, item => RecordingService.MarkRecordingAsSeen(item.UserName, item.RecordingId));
				});
			Receive<DeleteRecording>(msg => {
				Context.Handle(msg, item => RecordingService.DeleteRecording(item.UserName, item.RecordingId));
				});
			Receive<DeleteRecordingList>(msg => {
				Context.Handle(msg, item => RecordingService.DeleteList(item.UserName, item.IdList));
				});
			Receive<GetRecordingDates>(msg => {
				Context.Handle(msg, item => RecordingService.RecordingDates(item.UserName));
				});
			Receive<GetRecordingExcludedDates>(msg => {
				Context.Handle(msg, item => RecordingService.RecordingExcludedDates(item.UserName));
				});

        }
    }
}