using Chamberlain.Hub.HubApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications
{
    public abstract class VoiceNotification : RouteToHubGateway, IVoiceHardwareId
    {
        public string VoiceId { get; set; }
        public abstract string GetInformationText();
        public abstract string GetVoipInformationText();
    }
}
