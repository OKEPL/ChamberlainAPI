using Akka.Actor;

namespace Chamberlain.AppServer.Api.Contracts.Services
{
    public interface ITestService
    {
        void SimulateTrigger(string customerName, string itemCustomName, IActorRef ruleEngineActorRef);
    }
}