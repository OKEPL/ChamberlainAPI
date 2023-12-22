using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device;

namespace Chamberlain.AppServer.Api.Contracts.Commands.Devices
{
    public class GetDevices : HasUserName
    {
        public GetDevices(string userName)
            : base(userName)
        {
        }
    }

    public class GetDevicesThingNames : HasUserName
    {
        public GetDevicesThingNames(string userName)
            : base(userName)
        {
        }
    }

    public class GetDevice : HasUserName
    {
        public GetDevice(string userName, long deviceId)
            : base(userName)
        {
            DeviceId = deviceId;
        }

        public long DeviceId { get; }
    }

    public class GetAvaliableTriggers : HasUserName
    {
        public GetAvaliableTriggers(string userName, long deviceId)
            : base(userName)
        {
            DeviceId = deviceId;
        }

        public long DeviceId { get; }
    }

    public class UpdateDevice : HasUserName
    {
        public UpdateDevice(string userName, long deviceId, string deviceName)
            : base(userName)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
        }

        public long DeviceId { get; }

        public string DeviceName { get; }
    }

    public class DeleteDeviceByThingName : HasUserName
    {
        public DeleteDeviceByThingName(string userName, long thingId)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long ThingId { get; }
    }

    public class PairDevices : HasUserName
    {
        public PairDevices(string userName, PairDeviceModel model)
            : base(userName)
        {
            Model = model;
        }

        public PairDeviceModel Model { get; }
    }

    public class ControllerCommand : HasUserName
    {
        public ControllerCommand(string userName, long thingId, string command, string arg = null)
            : base(userName)
        {
            ThingId = thingId;
            Command = command;
            Arg = arg;
        }

        public string Arg { get; }

        public string Command { get; }

        public long ThingId { get; }
    }

    public class CancelDeviceCommand : HasUserName
    {
        public CancelDeviceCommand(string userName, long thingId)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long ThingId { get; }
    }

    public class SetDeviceValue : HasUserName
    {
        public SetDeviceValue(string userName, long itemId, string value)
            : base(userName)
        {
            ItemId = itemId;
            Value = value;
        }

        public long ItemId { get; }

        public string Value { get; }
    }

    public class HardResetIoTController : HasUserName
    {
        public HardResetIoTController(string userName, long? thingId = null)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long? ThingId { get; }
    }

    public class SoftRestartIoTController : HasUserName
    {
        public SoftRestartIoTController(string userName, long? thingId = null)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long? ThingId { get; }
    }

    public class HealZwaveNetwork : HasUserName
    {
        public HealZwaveNetwork(string userName, long? thingId = null)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long? ThingId { get; }
    }

    public class ForceDeviceUpdate : HasUserName
    {
        public ForceDeviceUpdate(string userName, long thingId)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long ThingId { get; }
    }

    public class GetDeviceTypeNames
    {
    }

    public class GetValueTypeNames
    {
    }

    public class GetGenreNames
    {
    }

    public class GetItemCategoryNames
    {
    }

    public class StartStopAudioPlugin : HasUserName
    {
        public StartStopAudioPlugin(string userName, long thingId, bool actionToggle)
            : base(userName)
        {
            ThingId = thingId;
            ActionToggle = actionToggle;
        }

        public bool ActionToggle { get; }

        public long ThingId { get; }
    }

    public class AudioData : HasUserName
    {
        public AudioData(string userName, byte[] data, long thingId) : base(userName)
        {
            Data = data;
            ThingId = thingId;
        }
        public byte[] Data { get; }

        public long ThingId { get; }
    }
}