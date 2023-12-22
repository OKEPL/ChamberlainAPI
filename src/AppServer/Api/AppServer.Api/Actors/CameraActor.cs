using Chamberlain.AppServer.Api.Contracts.Commands.Cameras;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class CameraActor : Receiver
    {
        [SetterProperty]
        public ICameraService CameraService { get; set; }

        public CameraActor()
        {
			Receive<GetCameraByItem>(msg => {
				Context.Handle(msg, item => CameraService.GetCameraByItem(item.UserName, item.ItemId));
			});
			Receive<GetCameraByThing>(msg => {
				Context.Handle(msg, item => CameraService.GetCameraByThing(item.UserName, item.ThingId));
			});
			Receive<GetCamerasExt>(msg => {
				Context.Handle(msg, item => CameraService.GetCamerasExt(item.UserName));
			});
			Receive<GetCameraByItemExt>(msg => {
				Context.Handle(msg, item => CameraService.GetCameraByItemExt(item.UserName, item.ItemId));
			});
			Receive<GetSupportedCameras>(msg => {
				Context.Handle(msg, item => CameraService.GetSupportedCameras());
			});
			Receive<UpdateCamera>(msg => {
				Context.Handle(msg, item => CameraService.UpdateCamera(item.UserName, item.Model));
			});
			Receive<DeleteCamera>(msg => {
				Context.Handle(msg, item => CameraService.DeleteCamera(item.UserName, item.ThingId));
			});
			Receive<TestRtspPort>(msg => {
				Context.Handle(msg, item => CameraService.TestRtspPort(item.Port, item.Ip));
			});
			Receive<TestHostPort>(msg => {
				Context.Handle(msg, item => CameraService.TestHostPort(item.Port, item.Ip));
			});

        }
    }
}