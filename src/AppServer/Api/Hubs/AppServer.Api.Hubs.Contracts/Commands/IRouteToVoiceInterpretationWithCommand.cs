using Chamberlain.Common.Content.VoiceCommands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands
{
    public interface IRouteToVoiceInterpretationWithCommand : IRouteToVoiceInterpretation
    {
        VoiceInterpretationRequest Request { get; set; }
    }
}