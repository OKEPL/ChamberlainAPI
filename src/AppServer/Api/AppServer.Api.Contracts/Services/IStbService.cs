using Akka.Actor;

namespace Chamberlain.AppServer.Api.Contracts.Services
{
    public interface IStbService
    {
        void StbLogin(string userName, string solocooLogin, IActorRef ActorRef);
    }
}