using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Scene;

namespace Chamberlain.AppServer.Api.Contracts.Commands.Scenes
{

    public class GetScenes : HasUserName
    {
        public GetScenes(string userName)
            : base(userName)
        {
        }
    }

    public class GetScene : HasUserName
    {
        public GetScene(string userName, long sceneId)
            : base(userName)
        {
            SceneId = sceneId;
        }

        public long SceneId { get; }
    }

    public class GetSceneDevices : HasUserName
    {
        public GetSceneDevices(string userName, long sceneId)
            : base(userName)
        {
            SceneId = sceneId;
        }

        public long SceneId { get; }

    }

    public class AddScene : HasUserName
    {
        public AddScene (string userName, SceneModel sceneModel)
            : base(userName)
        {
            SceneModel = sceneModel;
        }

        public SceneModel SceneModel{ get; }

    }

    public class UpdateScene : HasUserName
    {
        public UpdateScene(string userName, SceneModel sceneModel)
            : base(userName)
        {
            SceneModel = sceneModel;
        }

        public SceneModel SceneModel { get; }

    }

    public class AddThingToScene : HasUserName
    {
        public AddThingToScene(string userName, SceneThingModel sceneThingModel)
            : base(userName)
        {
            SceneThingModel = sceneThingModel;
        }

        public SceneThingModel SceneThingModel { get; }

    }

    public class DeleteScene : HasUserName
    {
        public DeleteScene(string userName, long sceneId) : base(userName)
        {
            SceneId = sceneId;
        }

        public long SceneId { get; }
    }

    public class DeleteDeviceFromScene : HasUserName
    {
        public DeleteDeviceFromScene(string userName, long sceneId, long thingId) : base(userName)
        {
            SceneId = sceneId;
            ThingId = thingId;
        }

        public long SceneId { get; }
        public long ThingId { get; }
    }
}
