using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device;

namespace Chamberlain.AppServer.Api.Contracts.Services
{
    public interface IDeviceService
    {
        void CancelDeviceCommand(string userName, long thingId);

        void ControllerCommand(string userName, long thingId, string command, string arg = null);

        void DeleteDeviceByThingId(string userName, long thingId);

        List<TriggerModel> GetAvaliableTriggers(string userName, long deviceId);

        DeviceModel GetDevice(string userName, long deviceId);

        List<DeviceModel> GetDevices(string userName);

        List<DeviceShortModel> GetDevicesThingNames(string userName);

        List<SettingModel> GetDeviceTypeNames();

        List<SettingModel> GetGenreNames();

        List<SettingModel> GetItemCategoryNames();

        List<SettingModel> GetValueTypeNames();

        void HardResetIoTController(string userName, long? thingId = null);

        void SoftRestartIoTController(string userName, long? thingId = null);

        void HealZwaveNetwork(string userName, long? thingId = null);

        void ForceDeviceUpdate(string userName, long thingId);

        void PairDevices(string userName, bool actionToggle);

        void PairDevices(string userName, bool actionToggle, long thingId);

        void SetDeviceValue(string userName, long itemId, string value);

        void StartStopAudioPlugin(string userName, long thingId, bool actionToogle);

        void UpdateDevice(string userName, long deviceId, string deviceName);
        void SendAudioData(string userName, long thingId, byte[] data);
    }
}