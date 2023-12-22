using Chamberlain.AppServer.Api.Contracts.Commands.Home;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class HomeActor : Receiver
    {
        [SetterProperty]
        public IHomeService HomeService { get; set; }

        public HomeActor()
        {
			Receive<GetStatus>(msg => {
				Context.Handle(msg, item => HomeService.GetStatus(item.UserName));
				});

			Receive<ClearEvents>(msg => {
				Context.Handle(msg, item => HomeService.ClearEvents(item.UserName));
				});

			Receive<GetEventsFromBegining>(msg => {
				Context.Handle(msg, item => HomeService.GetEventsFromBegining(item.UserName, item.Number, item.Language));
				});

			Receive<GetEventsFromName>(msg => {
				Context.Handle(msg, item => HomeService.GetEventsFromId(item.UserName, item.Number, item.LastSeen, item.Language));
				});

			Receive<GetCamerasWithImages>(msg => {
				Context.Handle(msg, item => HomeService.GetCamerasWithImages(item.UserName));
				});
				
			Receive< GetCameraImageByThingId>(msg => {
				Context.Handle(msg, item => HomeService.GetCameraImageByThingId(item.UserName, item.ThingId));
				});

			Receive<GetNewestEventsFromName>(msg => {
				Context.Handle(msg, item => HomeService.GetNewestEventsFromId(item.UserName, item.LastSeen, item.Language));
				});
        }
    }
}
