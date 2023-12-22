namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands
{
    public static class CameraDataTransfer
    {
        public class SendTsChunk
        {
            public SendTsChunk(string deviceKey, byte[] data, string chunkName, string extinf, long timestamp)
            {
                DeviceKey = deviceKey;
                Data = data;
                ChunkName = chunkName;
                Extinf = extinf;
                Timestamp = timestamp;
            }
            
            public byte[] Data { get; }
            public string DeviceKey { get; }
            public string ChunkName { get; }
            public string Extinf { get; }
            public long Timestamp { get; }
        }

        public class HasDeviceKey
        {
            public HasDeviceKey(string deviceKey)
            {
                DeviceKey = deviceKey;
            }

            public string DeviceKey { get; }
        }

        public class GetChunkSettings : HasDeviceKey
        {
            public GetChunkSettings(string deviceKey) : base(deviceKey)
            { }
        }

        public class GetChunkSettingsResponse
        {
            public GetChunkSettingsResponse(string mainPath, string previewPath, string recordingPath, int hubCameraListSize, int chunkSecondsLength)
            {
                MainPath = mainPath;
                PreviewPath = previewPath;
                RecordingPath = recordingPath;
                HubCameraListSize = hubCameraListSize;
                ChunkSecondsLength = chunkSecondsLength;
            }

            public string MainPath { get; }
            public string PreviewPath { get; }
            public string RecordingPath { get; }
            public int HubCameraListSize { get; }
            public int ChunkSecondsLength { get; }
        }

        public class UpdateCamera : HasDeviceKey
        {
            public UpdateCamera(string deviceKey) : base(deviceKey)
            { }
        }
    }
}