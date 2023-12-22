namespace Chamberlain.AppServer.Api.Contracts.Commands.Cameras
{
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device.Camera;
    
    public class UpdateCamera : HasUserName
    {
        public UpdateCamera(string userName, CameraSettingsModel model)
            : base(userName)
        {
            Model = model;
        }

        public CameraSettingsModel Model { get; }
    }

    public class DeleteCamera : HasUserName
    {
        public DeleteCamera(string userName, long thingId)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long ThingId { get; }
    }

    public class GetCameraByItem : HasUserName
    {
        public GetCameraByItem(string userName, long itemId)
            : base(userName)
        {
            ItemId = itemId;
        }

        public long ItemId { get; }
    }

    public class GetCameraByItemExt : HasUserName
    {
        public GetCameraByItemExt(string userName, long itemId)
            : base(userName)
        {
            ItemId = itemId;
        }

        public long ItemId { get; }
    }

    public class GetCameraByThing : HasUserName
    {
        public GetCameraByThing(string userName, long thingId)
            : base(userName)
        {
            ThingId = thingId;
        }

        public long ThingId { get; }
    }

    public class GetCamerasExt : HasUserName
    {
        public GetCamerasExt(string userName)
            : base(userName)
        {
        }
    }

    public class GetSupportedCameras
    {
    }

    public class TestHostPort : HasUserName
    {
        public TestHostPort(string userName, int port, string ip)
            : base(userName)
        {
            Port = port;
            Ip = ip;
        }

        public string Ip { get; }
        public int Port { get; }
    }

    public class TestRtspPort : HasUserName
    {
        public TestRtspPort(string userName, int port, string ip)
            : base(userName)
        {
            Port = port;
            Ip = ip;
        }

        public string Ip { get; }
        public int Port { get; }
    }
}