using Akka.Actor;
using Chamberlain.Common.Contracts.Enums;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Services
{
    public interface IVoiceUtilsService
    {
        void ReceiveSoundDownloadRequest(string text, IActorRef sender, string voiceUnitId);
        void GetDefaultSoundsList(string language, IActorRef actor, string requestId, string voiceUnitId);
        void SendGrammarUpdate(IActorRef actor, string userName , string voiceUnitId);
        void ReceivePartialRequestPart(IActorRef sender, int requestHash, int slicesCount, int sliceId, byte[] data, string voiceUnitId, string requestId, GoogleRecognitionContext context);
    }
}
