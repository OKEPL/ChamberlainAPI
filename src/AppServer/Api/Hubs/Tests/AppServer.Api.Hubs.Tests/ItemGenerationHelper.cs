using System.Linq;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Database.Persistency.Model;

namespace AppServer.Api.Hubs.Tests
{
    static class ItemGenerationHelper
    {
        private static int _iterator;

        public static HubWorkerCommand.ReportThing GetRequestForKnownDevice(string userName, string brand, string model)
        {
            using (var context = new Entities())
            {
                var device = context.KnownDevices.Single(x => x.BrandId == brand && x.ModelId == model);

                return new HubWorkerCommand.ReportThing($"{device.DeviceType}_Dummy{_iterator++}", new IoTThingData { DeviceStatusName = "test", DeviceTypeName = device.DeviceType, ManufacturerId = device.BrandId, ManufacturerName = device.BrandName, ProductId = device.ModelId, ProductName = device.ModelName, Settings = new BaseDeviceSettings { } })
                    { UserName = userName };
            }
        }


    }
}
