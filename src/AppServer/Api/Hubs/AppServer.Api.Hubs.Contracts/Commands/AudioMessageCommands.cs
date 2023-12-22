using Chamberlain.Hub.HubApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands
{
    public class AudioMessageCommands
    {
        public class SayAudioMessage : RouteToHubGateway
        {
            public SayAudioMessage(string messageText, string voiceId, string userName)
            {
                VoiceId = voiceId;
                MessageText = messageText;
                UserName = userName;
            }

            public string MessageText { get; set; }
            public string VoiceId { get; set; }
        }
    }
}
