using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Scene;
using Chamberlain.Database.Persistency.Model;

namespace Chamberlain.AppServer.Api.Helpers
{
    public static class SceneExtensions
    {
        public static SceneModel ToSceneModel(this Scene scene)
        {
            return new SceneModel()
            {
                Name = scene.Name,
                SceneId = scene.Id
            };

        }
    }
}
