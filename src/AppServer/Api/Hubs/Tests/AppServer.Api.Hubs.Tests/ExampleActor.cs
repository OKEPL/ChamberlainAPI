using Akka.Actor;
using Chamberlain.Common.Akka;
using Chamberlain.Common.Content.StructureMapContent;

namespace AppServer.Api.Hubs.Tests
{
    class ExampleActor : Receiver
    {
        public ExampleActor()
        {
            var hub = ObjectFactory.Container.GetInstance<IActorRef>("HubServiceActor");
        }
    }
}
