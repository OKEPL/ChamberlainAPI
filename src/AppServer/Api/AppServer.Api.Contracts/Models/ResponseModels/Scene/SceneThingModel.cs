namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Scene
{
    public class SceneThingModel
    {
        public SceneThingModel(long sceneId, long thingId)
        {
            SceneId = sceneId;
            ThingId = thingId;
        }

        public SceneThingModel()
        {
        }

        public long SceneId { get; set; }
        public long ThingId { get; set; }
    }
}