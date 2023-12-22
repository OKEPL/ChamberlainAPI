using Akka.Actor;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace AppServer.Api.Hubs.Tests
{
    [TestFixture]
    class AkkaReportingTests : TestKit
    {
        [Test]
        public void ExampleActorTest()
        {
            var example = Sys.ActorOf(Props.Create(() => new ExampleActor()));
        }
    }
}
