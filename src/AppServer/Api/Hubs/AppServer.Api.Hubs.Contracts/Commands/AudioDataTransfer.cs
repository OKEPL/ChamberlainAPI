using Chamberlain.Common.Akka;
using Chamberlain.Hub.HubApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands
{

    public class AudioDataTransfer
    {
        public class SendAudioData : Authorizable, ILargePacketMessage
        {
            public byte[] Data { get; set; }
            public string DeviceKey { get; set; }
            public int PartNumber { get; set; }
            public int TotalParts { get; set; }
            public long Timestamp { get; set; }

            public SendAudioData(string deviceKey, byte[] data)
            {
                DeviceKey = deviceKey;
                Data = data;
            }

            public ILargePacketMessage Copy()
            {
                return new SendAudioData(DeviceKey, Data)
                {
                    Token = Token,
                    Timestamp = Timestamp
                };
            }
        }

        public class ReceiveAudioData : IRouteToHubDeviceManager, ILargePacketMessage
        {
            public string UserName { get; set; }
            public byte[] Data { get; set; }
            public string DeviceKey { get; set; }
            public string Token { get; set; }
            public int PartNumber { get; set; }
            public int TotalParts { get; set; }
            public long Timestamp { get; set; }

            public ReceiveAudioData(string deviceKey, byte[] data, string userName)
            {
                DeviceKey = deviceKey;
                Data = data;
                UserName = userName;
            }

            public ILargePacketMessage Copy()
            {
                return new ReceiveAudioData(DeviceKey, Data, UserName)
                {
                    Token = Token,
                    Timestamp = Timestamp                    
                };
            }
        }

    }

}
