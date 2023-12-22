using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.Common.Akka;
using Chamberlain.Common.Content.VoiceCommands;
using Plugins.VoiceCommandExecutor.Communication;
using Serilog;
using VoiceApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Actors
{
    public class VoiceActor : Receiver
    {
        private IActorRef _commandInterpretationActorRef;

        public VoiceActor(IActorRef commandInterpretationActor)
        {
            _commandInterpretationActorRef = commandInterpretationActor;

            Receive<InterpretationCommand.SendRequest>(msg =>
			{
			    Log.Debug($"Voice Actor received interpretation request for text of {msg.Request.Text}");

			    var voiceCommandExecutor = Context.ActorOf(Props.Create(() => new VoiceCommandExecutorActor(_commandInterpretationActorRef)));
			    var result = voiceCommandExecutor.Ask<InterpretationCommand.InterpretationResponse>(msg).Result;
                
                Sender.Tell(result);
            });

        }
    }
}
