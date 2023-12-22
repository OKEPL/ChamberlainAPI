using Chamberlain.AppServer.Devices.Interfaces;
using Chamberlain.Database.Persistency.Model;
using System.Linq;

namespace Chamberlain.AppServer.Api.Hubs.Reporters
{
    public class ZmoteReporter : DefaultReporter
    {
        public ZmoteReporter(IDeviceManager deviceManager, string deviceKey, string userName)
            : base(deviceManager, deviceKey, userName)
        {
        }

        public ZmoteReporter(IDeviceManager deviceManager, string deviceKey, string userName, Entities context)
            : base(deviceManager, deviceKey, userName, context)
        {
        }

        protected override bool TryToRecognizeItem()
        {
            if (GetItem().Thing.KnownDeviceId == null) return false;

            var knownItem = Context.KnownItems.Where(ki => ki.KnownDeviceId == GetItem().Thing.KnownDeviceId).FirstOrDefault();

            if (knownItem == null) return false;

            GetItem().KnownItemId = knownItem.Id;

            Context.SaveChanges();

            return true;
        }
    }
}