using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Scene;

namespace Chamberlain.AppServer.Api.Contracts.Services
{
    public interface ISceneService
    {
        List<SceneModel> GetScenes(string userName);
        SceneModel GetScene(string userName, long sceneId);
        SceneModel AddScene(string userName, SceneModel sceneModel);
        SceneThingModel AddThingToScene(string userName, SceneThingModel sceneThingModel);
        void DeleteScene(string userName, long sceneId);
        void DeleteThingFromScene(string userName, long sceneId, long thingId);
        SceneModel UpdateScene(string userName, SceneModel sceneModel);
        List<SceneThingNamed> GetSceneThings(string userName, long sceneId);
    }
}