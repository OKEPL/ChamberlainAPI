using Chamberlain.Common.Akka;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands
{
    public class VoiceToCloudRequest : Authorizable
    {
        public string VoiceUnitId;
    }

    public class FromCloudToVoiceResponse : Authorizable
    {
        public string VoiceUnitId;
    }
}
