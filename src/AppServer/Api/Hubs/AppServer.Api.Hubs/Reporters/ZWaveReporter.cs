using Chamberlain.AppServer.Devices.Interfaces;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Domotica;
using Chamberlain.Database.Persistency.Model;
using System.Linq;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.AppServer.Api.Hubs.Helpers;
using Chamberlain.Common.Content.DataContracts.ZWave;
using Chamberlain.Common.Content.StructureMapContent;
using Chamberlain.Database.Persistency.Model.Extensions;
using Chamberlain.ExternalServices.RabbitMq;
using Chamberlain.Hub.HubApp.Contracts.Commands;
using Serilog;

namespace Chamberlain.AppServer.Api.Hubs.Reporters
{
    public class ZWaveReporter : DefaultReporter
    {
        public ZWaveReporter(IDeviceManager deviceManager, string deviceKey, string userName)
            : base(deviceManager, deviceKey, userName)
        {
        }

        public ZWaveReporter(IDeviceManager deviceManager, string deviceKey, string userName, Entities context)
            : base(deviceManager, deviceKey, userName, context)
        {
        }

        protected override bool TryToRecognizeThing()
        {
            return Recognizer.RecognizeZWaveDevice(GetThing(), true);
        }

        protected override void PostThingRecognized()
        {
            RabbitMqSender.SendMessage(new RabbitMqMessage
            {
                MessageType = MessageTypes.DeviceRecognizedMessageType,
                CustomerId = Customer.Id,
                Message = "Recognized device: " + $"{GetThing().GivenName ?? GetThing().BrandId + " " + GetThing().ModelId}".Trim(),
                LogToDb = true
            });

            ObjectFactory.Container.GetInstance<IHubNotifierService>().Notify(new HubDeviceManager.Notification
            {
                UserName = Customer.Username,
                DeviceAction = ZWaveDeviceActions.SetKnownDeviceConfiguration,
                DeviceKey = DeviceKey,
                ResponseType = HubDeviceManager.ResponseType.DeviceAction
            });

            if (Recognizer.KnownConfiguration?.MockItems != null)
            {
                AddMockItemsForZWaveDevice(GetThing(), Recognizer.KnownConfiguration);
            }          

            base.PostThingRecognized();
        }


        private void AddMockItemsForZWaveDevice(Thing thing, ZWaveKnownDeviceParameters deviceParameters)
        {
            var thingUpdated = false;

            foreach (var mock in deviceParameters.MockItems)
            {
                var itemNativeKey = ItemNativeKeyModel.FromThingKeyModel(thing.GetNativeKeyModel(), mock.MockItemCommandClass, mock.MockItemValueIndex);
                var item = thing.Items.FirstOrDefault(i =>
                {
                    var result = false;
                    var settings = XmlSerialization.FromString<ZWaveMockItemSettings>(i.Settings, true);
                    if (itemNativeKey != null)
                    {
                        result = i.NativeKey == itemNativeKey.ToString();
                    }
                    else if (settings != null)
                    {
                        return settings.MockItemType == mock.MockItemType;
                    }

                    return result;
                });

                if (item != null) continue;

                MockItemCreationHelper.AddZWaveMockItem(Context, thing, mock, itemNativeKey?.ToString());

                thingUpdated = true;
            }

            if (thingUpdated)
                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.ThingUpdatedMessageType, 0, null, null, thing.Id.ToString()));
        }
    }
}