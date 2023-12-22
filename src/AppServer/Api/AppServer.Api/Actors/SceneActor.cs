using Chamberlain.AppServer.Api.Contracts.Commands.Scenes;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class SceneActor : Receiver
    {
        [SetterProperty]
        public ISceneService SceneService { get; set; }

        public SceneActor()
        {
			Receive<GetScenes>(msg => {
				Context.Handle(msg, item => SceneService.GetScenes(item.UserName));
				});
			Receive<GetScene>(msg => {
				Context.Handle(msg, item => SceneService.GetScene(item.UserName, item.SceneId));
				});
			Receive<AddScene>(msg => {
				Context.Handle(msg, item => SceneService.AddScene(item.UserName, item.SceneModel));
				});
			Receive<AddThingToScene>(msg => {
				Context.Handle(msg, item => SceneService.AddThingToScene(item.UserName, item.SceneThingModel));
				});
			Receive<GetSceneDevices>(msg => {
				Context.Handle(msg, item => SceneService.GetSceneThings(item.UserName, item.SceneId));
				});
			Receive<DeleteScene>(msg => {
				Context.Handle(msg, item => SceneService.DeleteScene(item.UserName, item.SceneId));
				});
			Receive< DeleteDeviceFromScene>(msg => {
				Context.Handle(msg, item => SceneService.DeleteThingFromScene(item.UserName, item.SceneId, item.ThingId));
				});
			Receive<UpdateScene>(msg => {
				Context.Handle(msg, item => SceneService.UpdateScene(item.UserName, item.SceneModel));
				});
        }
    }
}