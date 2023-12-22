using System.Collections.Generic;
using System.Threading.Tasks;
using Chamberlain.AppServer.Api.Contracts.Commands.Scenes;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Scene;
using Chamberlain.AppServer.Api.Endpoint.Helpers;
using Chamberlain.Common.Akka;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    using global::AppServer.Api.Endpoint.Controllers;
    using Microsoft.AspNetCore.Mvc;
    /// <inheritdoc />
    /// <summary>
    /// Scene Controller class
    /// </summary>
    [Route("scenes")]
    public class SceneController : ChamberlainBaseController
    {
        /// <summary>
        /// Gets scene of id for current user.
        /// </summary>
        /// <param name="id">
        /// RuleId of scene to get.
        /// </param>
        /// <returns>
        /// Scene model describing wanted scene.
        /// </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SceneModel), 200)]
        public async Task<IActionResult> GetScene(long id)
        {
            var result = await SystemActors.SceneActor.Execute<GetScene, SceneModel>(new GetScene(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all scenes from current user.
        /// </summary>
        /// <returns>
        /// List of scenes for current user.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<SceneModel>), 200)]
        public async Task<IActionResult> GetScenes()
        {
            var result = await SystemActors.SceneActor.Execute<GetScenes, List<SceneModel>>(new GetScenes(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Deletes scene of id for current user.
        /// </summary>
        /// <param name="id">
        /// SceneId of scene to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteScene(long id)
        {
            await SystemActors.SceneActor.Execute(new DeleteScene(User.Identity.Name, id));
            return NoContent();
        }

        /// <summary>
        /// Adds scene for current user.
        /// </summary>
        /// <param name="sceneModel">
        /// SceneModel of scene to add.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(SceneModel), 200)]
        public async Task<IActionResult> AddScene([FromBody]SceneModel sceneModel)
        {
            var result = await SystemActors.SceneActor.Execute<AddScene, SceneModel>(new AddScene(User.Identity.Name, sceneModel));
            return Ok(result);
        }

        /// <summary>
        /// Update scene of given sceneId with rest of the properties.
        /// </summary>
        /// <param name="sceneModel">
        /// Rule model describing new scene. SceneId have to match updated one.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(typeof(SceneModel), 200)]
        public async Task<IActionResult> UpdateScene([FromBody] SceneModel sceneModel)
        {
            var result = await SystemActors.SceneActor.Execute<UpdateScene, SceneModel>(new UpdateScene(User.Identity.Name, sceneModel));
            return Ok(result);
        }

        /// <summary>
        /// Adds scene for selected scene.
        /// </summary>
        /// /// <param name="id">
        /// SceneId of scene to add thing.
        /// </param>
        /// <param name="sceneThingModel">
        /// Model describing thing to add.
        /// </param>
        /// <returns>
        /// Returns 201 and SceneDeviceModel if created successfully.
        /// </returns>
        [HttpPost("{id}/things")]
        [ProducesResponseType(typeof(SceneThingModel), 201)]
        public async Task<IActionResult> AddThingToScene(long id, [FromBody] SceneThingModel sceneThingModel)
        {
            var result = await SystemActors.SceneActor.Execute<AddThingToScene, SceneThingModel>(new AddThingToScene(User.Identity.Name, new SceneThingModel(id, sceneThingModel.ThingId)));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets list of things from scene of id for current user.
        /// </summary>
        /// <param name="id">
        /// SceneId of scene to get things from.
        /// </param>
        /// <returns>
        /// List of things
        /// </returns>
        [HttpGet("{id}/things")]
        [ProducesResponseType(typeof(List<SceneThingNamed>), 200)]
        public async Task<IActionResult> GetSceneThings(long id)
        {
            var result = await SystemActors.SceneActor.Execute<GetSceneDevices, List<SceneThingNamed>>(new GetSceneDevices(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Deletes thing from scene of id for current user.
        /// </summary>
        /// <param name="sceneId">
        /// SceneId of scene to delete.
        /// </param>
        /// <param name="thingId">
        /// ThingId of thing to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{sceneId}/things/{thingId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteThingFromScene(long sceneId, long thingId)
        {
            await SystemActors.SceneActor.Execute(new DeleteDeviceFromScene(User.Identity.Name, sceneId, thingId));
            return NoContent();
        }
    }
}
