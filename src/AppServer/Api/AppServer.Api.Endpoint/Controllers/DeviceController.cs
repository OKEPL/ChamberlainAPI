using System.Collections.Generic;
using System.Threading.Tasks;
using Chamberlain.AppServer.Api.Contracts.Commands.Devices;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device;
using Chamberlain.AppServer.Api.Endpoint.Helpers;
using Chamberlain.Common.Akka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppServer.Api.Endpoint.Controllers;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Device controller class.
    /// </summary>
    [Route("devices")]
    public class DeviceController : ChamberlainBaseController
    {
        /// <summary>
        /// Cancels device command for given ThingId.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of device to cancel command.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("cancelCommand")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> CancelDeviceCommand([FromBody] long thingId)
        {
            await SystemActors.DeviceActor.Execute(new CancelDeviceCommand(User.Identity.Name, thingId));
            return NoContent();
        }

        /// <summary>
        /// Calls given command with args for certain thing.
        /// </summary>
        /// <param name="model">
        /// Model with needed args, commands and thingId to call on.</param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("controllerCommand")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ControllerCommand([FromBody] ControllerCommandModel model)
        {
            await SystemActors.DeviceActor.Execute(new ControllerCommand(User.Identity.Name, model.ThingId, model.Command, model.Arg));
            return NoContent();
        }

        /// <summary>
        ///  Delete device using it's thingId.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of device to delete.</param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{thingId:long}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteDeviceByThingId(long thingId)
        {
            await SystemActors.DeviceActor.Execute(new DeleteDeviceByThingName(User.Identity.Name, thingId));
            return NoContent();
        }

        /// <summary>
        /// Gets device using given thingId.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of device.
        /// </param>
        /// <returns>
        /// Device model of given thingId.
        /// </returns>
        [HttpGet("{thingId:long}")]
        [ProducesResponseType(typeof(DeviceModel), 200)]
        public async Task<IActionResult> Get(long thingId)
        {
            var result = await SystemActors.DeviceActor.Execute<GetDevice, DeviceModel>(new GetDevice(User.Identity.Name, thingId));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all devices of user.
        /// </summary>
        /// <returns>
        /// List of users devices.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<DeviceModel>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await SystemActors.DeviceActor.Execute<GetDevices, List<DeviceModel>>(new GetDevices(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all devices id and name of user.
        /// </summary>
        /// <returns>
        /// List of users devices.
        /// </returns>
        [HttpGet("thingNames")]
        [ProducesResponseType(typeof(List<DeviceShortModel>), 200)]
        public async Task<IActionResult> GetDeviceThingNames()
        {
            var result = await SystemActors.DeviceActor.Execute<GetDevicesThingNames, List<DeviceShortModel>>(new GetDevicesThingNames(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets id and name of possible  device types.
        /// </summary>
        /// <returns>
        /// List of id, name of possible device types.
        /// </returns>
        [AllowAnonymous]
        [HttpGet("getDeviceTypeNames")]
        [ProducesResponseType(typeof(List<SettingModel>), 200)]
        public async Task<IActionResult> GetDeviceTypeNames()
        {
            var result = await SystemActors.DeviceActor.Execute<GetDeviceTypeNames, List<SettingModel>>(new GetDeviceTypeNames());
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets id and name of possible  device genres.
        /// </summary>
        /// <returns>
        /// List of id, name of possible device genres.
        /// </returns>
        [AllowAnonymous]
        [HttpGet("getGenreNames")]
        [ProducesResponseType(typeof(SettingModel), 200)]
        public async Task<IActionResult> GetGenreNames()
        {
            var result = await SystemActors.DeviceActor.Execute<GetGenreNames, List<SettingModel>>(new GetGenreNames());
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets id and name of possible  device categories.
        /// </summary>
        /// <returns>
        /// List of id, name of possible device categories.
        /// </returns>
        [AllowAnonymous]
        [HttpGet("getItemCategoryNames")]
        [ProducesResponseType(typeof(SettingModel), 200)]
        public async Task<IActionResult> GetItemCategoryNames()
        {
            var result = await SystemActors.DeviceActor.Execute<GetItemCategoryNames, List<SettingModel>>(new GetItemCategoryNames());
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets avaialble triggers for given thingId.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of thing we want to get triggers.</param>
        /// <returns>
        /// List of available triggers.
        /// </returns>
        [HttpGet("avaliableTriggers/{thingId:long}")]
        [ProducesResponseType(typeof(List<TriggerModel>), 200)]
        public async Task<IActionResult> GetTriggers(long thingId)
        {
            var result = await SystemActors.DeviceActor.Execute<GetAvaliableTriggers, List<TriggerModel>>(new GetAvaliableTriggers(User.Identity.Name, thingId));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets id and name of possible  device value types names.
        /// </summary>
        /// <returns>
        /// List of id, name of possible device value types names.
        /// </returns>
        [AllowAnonymous]
        [HttpGet("getValueTypeNames")]
        [ProducesResponseType(typeof(List<SettingModel>), 200)]
        public async Task<IActionResult> GetValueTypeNames()
        {
            var result = await SystemActors.DeviceActor.Execute<GetValueTypeNames, List<SettingModel>>(new GetValueTypeNames());
            return new ObjectResult(result);
        }

        /// <summary>
        /// Hard reset IoT controller using thingId if given, else creates default thing.
        /// </summary>
        /// <param name="thingId">
        /// ThingId to reset.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("hardResetIoTController")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> HardResetIoTController([FromBody] HardResetControllerModel model)
        {
            await SystemActors.DeviceActor.Execute(new HardResetIoTController(User.Identity.Name, model.ThingId));
            return NoContent();
        }

        /// <summary>
        /// Soft, safe restart (no risk of any consequences or device loss) of IoT controller using thingId if given, else creates default thing.
        /// </summary>
        /// <param name="thingId">
        /// ThingId to restart.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("softRestartIoTController")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SoftRestartIoTController([FromBody] SoftRestartControllerModel model)
        {
            await SystemActors.DeviceActor.Execute(new SoftRestartIoTController(User.Identity.Name, model.ThingId));
            return NoContent();
        }

        /// <summary>
        /// Performs healing of the network, may take a while. There is no risk or anything, you can launch it every time something acts weird. Heals network with given IoT controller using thingId if given, else creates default thing.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of controller to heal the network with.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("healZwaveNetwork")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> HealZwaveNetwork([FromBody] HealZWaveNetworkModel model)
        {
            await SystemActors.DeviceActor.Execute(new HealZwaveNetwork(User.Identity.Name, model.ThingId));
            return NoContent();
        }

        /// <summary>
        /// Forces device to send updates.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of the device.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("forceDeviceUpdate")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ForceDeviceUpdate([FromBody] ForceDeviceUpdateModel model)
        {
            await SystemActors.DeviceActor.Execute(new ForceDeviceUpdate(User.Identity.Name, model.ThingId));
            return NoContent();
        }

        /// <summary>
        /// Pair device of given thingId else takes first active.
        /// </summary>
        /// <param name="model">
        /// Model used to pair device.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("pairDevices")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PairDevices([FromBody] PairDeviceModel model)
        {
            await SystemActors.DeviceActor.Execute(new PairDevices(User.Identity.Name, model));
            return NoContent();
        }

        /// <summary>
        /// Set device Id with given value.
        /// </summary>
        /// <param name="model">
        /// Model of itemId and value.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("setDeviceValue")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SetDeviceValue([FromBody] SetDeviceValueModel model)
        {
            await SystemActors.DeviceActor.Execute(new SetDeviceValue(User.Identity.Name, model.ItemId, model.Value));
            return NoContent();
        }

        /// <summary>
        /// Start or stop audio plugin depends on actionToggle parameter.
        /// </summary>
        /// <param name="thingId">
        /// ThingId of thing to toggle.
        /// </param>
        /// <param name="actionToggle">
        /// If true will startAudioPlugin else will stop.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpGet("startStopAudioPlugin")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> StartStopAudioPlugin(long thingId, bool actionToggle)
        {
            await SystemActors.DeviceActor.Execute(new StartStopAudioPlugin(User.Identity.Name, thingId, actionToggle));
            return NoContent();
        }
        /// <summary>
        /// Send audio chunk to specified device
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("sendAudio")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SendAudio([FromBody]AudioData data)
        {
            await SystemActors.DeviceActor.Execute(new AudioData(User.Identity.Name, data.Data, data.ThingId));
            return NoContent();
        }

        /// <summary>
        /// Update device name using thingId.
        /// </summary>
        /// <param name="model">
        /// Model with both thingId of thing we want to update and new name.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateDevice([FromBody] UpdateDeviceModel model)
        {
            if (string.IsNullOrEmpty(model.DeviceName))
                return BadRequest(new { deviceName = "isRequired" });

            await SystemActors.DeviceActor.Execute(new UpdateDevice(User.Identity.Name, model.ThingId, model.DeviceName));
            return NoContent();
        }
    }
}