using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using AppServer.Api.Hubs.Tests;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.AppServer.Api.Hubs.Services;
using Chamberlain.AppServer.Devices.Interfaces;
using Chamberlain.Common.Content.StructureMapContent;
using Chamberlain.Database.Persistency.Model;
using Moq;
using NUnit.Framework;

namespace Chamberlain.AppServer.Api.Hubs.Tests
{
    [TestFixture]
    class ReportingTests
    {
        private Customer DummyCustomer => new Customer { Password = "123", Email = "okeokeoke.pl", IsActive = true, Username = "dummyCustomer123reporting", Language = "pl" };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ObjectFactory.Container.Configure(x => x.For<IHubService>().Use<HubService>().Singleton());
            ObjectFactory.Container.Configure(x => x.For<IHubNotifierService>().Use(new Mock<IHubNotifierService>().Object));
            ObjectFactory.Container.Configure(x => x.For<IDeviceManager>().Use(new Mock<IDeviceManager>().Object));
        }

        [SetUp]
        [TearDown]
        public void SetUp()
        {
            using (var context = new Entities())
            {
                context.Customers.RemoveRange(context.Customers.Where(x => x.Username == DummyCustomer.Username));
                context.Customers.Add(DummyCustomer);
                context.SaveChanges();
            }
        }

        [Test]
        [Explicit]
        public void ReportNewThing_ShouldSucceed()
        {
            var brandName = "Philips";
            var modelName = "LWB010";

            var id = ObjectFactory.Container.GetInstance<IHubService>().ReportThing(ItemGenerationHelper.GetRequestForKnownDevice(DummyCustomer.Username, brandName, modelName));

            Assert.NotNull(id);
        }

        [Test]
        [Explicit]
        public void ReportManyThings_ShouldSucceed()
        {
            var brandName = "Nest";
            var modelName = "protect";

            for (int i = 0; i < 5; i++)
            {
                ObjectFactory.Container.GetInstance<IHubService>().ReportThing(ItemGenerationHelper.GetRequestForKnownDevice(DummyCustomer.Username, brandName, modelName));
            }

            using (var context = new Entities())
            {
                var count = context.Things.Count(x => x.Customer.Username == DummyCustomer.Username && x.ModelId == modelName);
                Assert.AreEqual(5, count);
            }
        }

        [Test]
        [Explicit]
        public void ReportManyMoreThings_ShouldSucceed()
        {
            var brandName = "Nest";
            var modelName = "protect";

            for (int i = 0; i < 100; i++)
            {
                ObjectFactory.Container.GetInstance<IHubService>().ReportThing(ItemGenerationHelper.GetRequestForKnownDevice(DummyCustomer.Username, brandName, modelName));
            }

            using (var context = new Entities())
            {
                var count = context.Things.Count(x => x.Customer.Username == DummyCustomer.Username && x.ModelId == modelName);
                Assert.AreEqual(100, count);
            }
        }

        [Test]
        [Explicit]
        public void ReportManyThings_OneSeparateTask_ShouldSucceed()
        {
            var brandName = "Nest";
            var modelName = "thermostat";

            var tasks = new List<Task>();

            for (var taskIndex = 0; taskIndex < 1; taskIndex++)
            {
                tasks.Add(Task.Run(() => {
                        ObjectFactory.Container.GetInstance<IHubService>().ReportThing(ItemGenerationHelper.GetRequestForKnownDevice(DummyCustomer.Username, brandName, modelName));
                }));
            }

            foreach (var task in tasks)
                task.Wait();

            using (var context = new Entities())
            {
                var count = context.Things.Count(x => x.Customer.Username == DummyCustomer.Username && x.ModelId == modelName);
                Assert.AreEqual(1, count);
            }
        }

        [Test]
        [Explicit]
        public void ReportManyThings_TwoSeparateTasks_ShouldSucceed()
        {
            var brandName = "Nest";
            var modelName = "thermostat";

            var tasks = new List<Task>();


            for (var taskIndex = 0; taskIndex < 2; taskIndex++)
            {
                tasks.Add(Task.Run(() => {
                    ObjectFactory.Container.GetInstance<IHubService>().ReportThing(ItemGenerationHelper.GetRequestForKnownDevice(DummyCustomer.Username, brandName, modelName));
                }));
            }

            foreach (var task in tasks)
                task.Wait();

            using (var context = new Entities())
            {
                var count = context.Things.Count(x => x.Customer.Username == DummyCustomer.Username && x.ModelId == modelName);
                Assert.AreEqual(2, count);
            }
        }
    }
}
