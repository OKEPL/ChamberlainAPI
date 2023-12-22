namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Cameras;
    using Chamberlain.AppServer.Api.Contracts.DataTransfer;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device.Camera;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Camera Controller class.
    /// </summary>
    [Route("camera")]
    public class CameraController : ChamberlainBaseController
    {
        /// <summary>
        /// Delete camera of given thing ID.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of camera to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{thingId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteCamera(long thingId)
        {
            await SystemActors.CameraActor.Execute(new DeleteCamera(User.Identity.Name, thingId));
            return NoContent();
        }

        /// <summary>
        /// Gets extended information about camera given by itemId.
        /// </summary>
        /// <param name="itemId">
        /// ItemId of camera we would like to get.
        /// </param>
        /// <returns>
        /// Single CameraSettingsModel.
        /// </returns>
        [HttpGet("extended/{itemId:long}")]
        [ProducesResponseType(typeof(CameraSettingsModel), 200)]
        public async Task<IActionResult> GetCameraByItemExt(long itemId)
        {
            var result = await SystemActors.CameraActor.Execute<GetCameraByItemExt, CameraSettingsModel>(new GetCameraByItemExt(User.Identity.Name, itemId));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets single camera of logged user using itemId.
        /// </summary>
        /// <param name="itemId">
        /// ItemId of camera.
        /// </param>
        /// <returns>
        /// Single camera model.
        /// </returns>
        [HttpGet("itemId/{itemId}")]
        [ProducesResponseType(typeof(CameraModel), 200)]
        public async Task<IActionResult> GetCameraByItemId(long itemId)
        {
            var result = await SystemActors.CameraActor.Execute<GetCameraByItem, CameraModel>(new GetCameraByItem(User.Identity.Name, itemId));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets single camera of logged user using thingId.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of camera.
        /// </param>
        /// <returns>
        /// Single camera model.
        /// </returns>
        [HttpGet("thingId/{thingId}")]
        [ProducesResponseType(typeof(CameraModel), 200)]
        public async Task<IActionResult> GetCameraByThingId(long thingId)
        {
            var result = await SystemActors.CameraActor.Execute<GetCameraByThing, CameraModel>(new GetCameraByThing(User.Identity.Name, thingId));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets extended information about all cameras of logged user.
        /// </summary>
        /// <returns>
        /// List of CameraSettingsModel</returns>
        [HttpGet("extended")]
        [ProducesResponseType(typeof(List<CameraSettingsModel>), 200)]
        public async Task<IActionResult> GetCamerasExt()
        {
            var result = await SystemActors.CameraActor.Execute<GetCamerasExt, List<CameraSettingsModel>>(new GetCamerasExt(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets list of supported cameras of logged user.
        /// </summary>
        /// <returns>
        /// List of SupportedCameraBrandModel</returns>
        [HttpGet("supported")]
        [ProducesResponseType(typeof(List<SupportedCameraBrandModel>), 200)]
        public async Task<IActionResult> GetSupportedCameras()
        {
            var result = await SystemActors.CameraActor.Execute<GetSupportedCameras, List<SupportedCameraBrandModel>>(new GetSupportedCameras());
            return new ObjectResult(result);
        }

        /// <summary>
        /// Test host ip:port connection.
        /// </summary>
        /// <param name="port">
        /// Port which should be tested
        /// </param>
        /// <param name="ip">
        /// Ip which should be tested
        /// </param>
        /// <returns>
        /// Exist model
        /// </returns>
        [HttpGet]
        [Route("testPort/{port:int}/{ip}")]
        [ProducesResponseType(typeof(CheckResultModel), 200)]
        public async Task<IActionResult> TestHostPort(int port, string ip)
        {
            var result = await SystemActors.CameraActor.Execute<TestHostPort, CheckResultModel>(new TestHostPort(User.Identity.Name, port, ip));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Test rtsp ip:port connection.
        /// </summary>
        /// <param name="port">
        /// Port which should be tested
        /// </param>
        /// <param name="ip">
        /// Ip which should be tested
        /// </param>
        /// <returns>
        /// Exist model
        /// </returns>
        [HttpGet]
        [Route("rtsp/{port:int}/{ip}")]
        [ProducesResponseType(typeof(CheckResultModel), 200)]
        public async Task<IActionResult> TestRtspPort(int port, string ip)
        {
            var result = await SystemActors.CameraActor.Execute<TestRtspPort, CheckResultModel>(new TestRtspPort(User.Identity.Name, port, ip));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Update camera settings using new model.
        /// </summary>
        /// <param name="cameraSettingsModel">
        /// CameraSettingsModel with new params, ItemId is required to find previous camera settings.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateCamera([FromBody] CameraSettingsModel cameraSettingsModel)
        {
            await SystemActors.CameraActor.Execute(new UpdateCamera(User.Identity.Name, cameraSettingsModel));
            return NoContent();
        }
    }
}