namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Scene
{
    public class SceneModel
    {
        public long SceneId { get; set; }
        public string Name { get; set; }

        public SceneModel(long sceneId, string name)
        {
            SceneId = sceneId;
            Name = name;
        }

        public SceneModel()
        {
        }
    }
}
