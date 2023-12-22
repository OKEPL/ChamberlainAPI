using Chamberlain.AppServer.Devices.Interfaces;
using Chamberlain.Common.Content;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Domotica;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.ExternalServices.RabbitMq;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Chamberlain.AppServer.Devices;
using Chamberlain.Common.Content.StructureMapContent;
using Chamberlain.Common.Settings;
using Chamberlain.Plugins.DeviceManager.Communication;
using PredefinedRulesManager.Interfaces;
using static Chamberlain.AppServer.Api.Hubs.Contracts.Commands.HubWorkerCommand;

namespace Chamberlain.AppServer.Api.Hubs.Reporters
{
    public class DefaultReporter : Disposable, IReporter
    {
        protected readonly IActorRef DeviceManagerActorRef;
        protected readonly IDeviceManager DeviceManager;
        protected readonly Entities Context;
        protected readonly string DeviceKey;
        protected readonly string UserName;
        protected readonly bool ShouldDisposeContext;

        public DefaultReporter(IDeviceManager deviceManager, string deviceKey, string userName)
        {
            DeviceManager = deviceManager;
            DeviceManagerActorRef = deviceManager.GetActorRef();

            Context = new Entities();
            Recognizer = new KnownDeviceRecognizer();
            ShouldDisposeContext = true;
            DeviceKey = deviceKey;
            UserName = userName;
        }

        public DefaultReporter(IDeviceManager deviceManager, string deviceKey, string userName, Entities context) : this(deviceManager, deviceKey, userName)
        {
            Context = context;
            ShouldDisposeContext = false;         
        }

        public long ReportThing(IoTThingData deviceData)
        {
            Log.Information($"Default reporter: Reporting thing {deviceData.ProductName}");
            Log.Debug($"Default reporter: DeviceKey: {DeviceKey} ManufacturerName: {deviceData.ManufacturerName} ManufacturerId: {deviceData.ManufacturerId} ProductId: {deviceData.ProductId}");

            var thing = Customer.Things.FirstOrDefault(w => w.NativeKey == DeviceKey);

            return thing == null ? AddThing(deviceData) : UpdateThing(thing, deviceData);
        }

        public long ReportItem(ReportItem request)
        {
            Log.Information($"Default reporter: Report item called for {request.DeviceData.NativeName}");
            Log.Debug($"Default reporter: DeviceKey: {DeviceKey} CustomName: {request.DeviceData.CustomName} Value: {request.DeviceData.Value}");

            var item = Customer.Things.SelectMany(t => t.Items).FirstOrDefault(i => i.NativeKey == DeviceKey);

            return item == null ? AddItem(request.DeviceData) : UpdateItem(item, request);
        }

        public void ReportDeviceReadyForCommands()
        {
            Log.Debug($"Default reporter: Device {GetThing().NativeName} with key {DeviceKey} was declared as ready for commands.");

            RecognizeThingIfNeeded();

            PostReadyForCommands();
        }

        #region Add/Update item
        private long AddItem(IoTItemData itemData)
        {
            Log.Debug($"Default reporter: Adding new item {itemData.CustomName}, DeviceKey: {DeviceKey}");

            var thingId = GetThing().Id;
            RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.ThingUpdatedMessageType, 0,
                null, null, thingId.ToString()));

            var thingKeyModel = ThingNativeKeyModel.FromNativeKey(DeviceKey);
            var item = new Item
            {
                NativeKey = DeviceKey,
                NativeValue = itemData.Value,
                CustomName = itemData.NativeName,
                NativeName = itemData.NativeName,
                Type = thingKeyModel.DatabaseDeviceType,
                ThingId = thingId,
                LastSeen = DateTime.UtcNow,
            };

            if (itemData.Settings is IoTItemSettings settings)
            {
                item.Settings = XmlSerialization.ToString(settings);
            }

            Context.Items.Add(item);
            Context.SaveChanges();

            PostAddItem();

            Log.Debug($"Default reporter: Added new item {itemData.CustomName}, DeviceKey: {DeviceKey}, Id: {item.Id}");
            return item.Id;
        }

        protected long UpdateItem(Item item, ReportItem request)
        {
            Log.Debug($"Default reporter: Item {item.NativeName} (ID:{item.Id}, DK: {DeviceKey}) changed value from {item.NativeValue} to {request.DeviceData.Value}");

            if (!string.IsNullOrEmpty(request.DeviceData.Value))
            {
                item.NativeValue = request.DeviceData.Value;
                item.LastSeen = DateTime.UtcNow;
                Context.SaveChanges();
            }

            DeviceManagerActorRef.Tell(new UpdateDeviceNativeData(item.Id, request));
            PostUpdateItem();

            return item.Id;
        }

        #endregion

        #region Add/Update thing
        private long AddThing(IoTThingData thingData)
        {
            Log.Debug($"Default reporter: Adding new thing {thingData.ProductName}, DeviceKey: {DeviceKey}");

            var thing = new Thing
            {
                CustomerId = Customer.Id,
                ModelId = thingData.ProductId,
                BrandId = thingData.ManufacturerId,
                GivenName = thingData.DetermineThingName(),
                NativeName = thingData.DeviceTypeName,
                NativeKey = DeviceKey
            };

            if (thingData.Settings is IoTThingSettings settings)
                thing.Settings = XmlSerialization.ToString(settings);

            Context.Things.Add(thing);
            _thing = thing;
            Context.SaveChanges();
            PostAddThing();

            Log.Debug($"Default reporter: New thing added with id {_thing.Id}");

            return GetThing().Id;
        }

        private long UpdateThing(Thing thing, IoTThingData thingData)
        {
            Log.Debug($"Default reporter: Thing {thing.GivenName} (ID: {thing.Id}, DK: {DeviceKey}) updates data");

            if (!string.IsNullOrEmpty(thingData.ManufacturerId))
                thing.BrandId = thingData.ManufacturerId;
            if (!string.IsNullOrEmpty(thingData.ProductId))
                thing.ModelId = thingData.ProductId;
            if (string.IsNullOrEmpty(thing.GivenName))
                thing.GivenName = thingData.DetermineThingName();
            if (string.IsNullOrEmpty(thing.NativeName))
                thing.NativeName = thingData.DeviceTypeName;

            Context.SaveChanges();
            PostUpdateThing();
            return thing.Id;
        }
        #endregion

        public void RecognizeThingIfNeeded()
        {

            if (GetThing().KnownDevice == null && TryToRecognizeThing())
            {
                Log.Debug($"Default reporter: Thing {GetThing().GivenName} (ID: {GetThing().Id}, DK: {DeviceKey}) will be recognized.");
                PostThingRecognized();
            }
        }

        public void RecognizeItemIfNeeded()
        {
            if (GetItem().KnownItem == null && TryToRecognizeItem())
            {
                Log.Debug($"Default reporter: Item {GetItem().NativeName} (ID: {GetItem().Id}, DK: {DeviceKey}) will be recognized.");
                PostItemRecognized();
            }
        }

        #region Overrideable virtuals

        protected virtual bool TryToRecognizeThing()
        {
            return Recognizer.RecognizeOtherDevice(Context, GetThing());
        }

        protected virtual bool TryToRecognizeItem()
        {
            return Recognizer.RecognizeItem(Context, GetItem());
        }

        //Notice: always call "Post" base methods at the end when overriding them.

        protected virtual void PostAddItem()
        {
            if (GetItem().Thing.KnownDeviceId != null)
                RecognizeItemIfNeeded();
        }

        protected virtual void PostUpdateItem()
        {

        }

        protected virtual void PostAddThing()
        {

        }

        protected virtual void PostUpdateThing()
        {

        }
        protected virtual void PostReadyForCommands()
        {

        }

        protected virtual void PostThingRecognized()
        {
            var rabbitMsg = new RabbitMqMessage
            {
                MessageType = MessageTypes.DeviceRecognizedMessageType,
                CustomerId = Customer.Id,
                Message = "Default reporter: Recognized device: " + $"{GetThing().GivenName ?? GetThing().BrandId + " " + GetThing().ModelId}".Trim(),
                LogToDb = true
            };

            RabbitMqSender.SendMessage(rabbitMsg);
            Log.Debug($"{rabbitMsg.Message} [known config exists:{Recognizer.KnownConfiguration != null}, mock items exists:{Recognizer.KnownConfiguration?.MockItems != null}]");

            foreach (var item in GetThing().Items)
                using (var reporter = ReporterFactory.GetReporter(DeviceManager, item.NativeKey, UserName, Context))
                    reporter.RecognizeItemIfNeeded();
        }

        protected virtual void PostItemRecognized()
        {
            ObjectFactory.Container.GetInstance<IPredefinedRulesManagerPlugin>().InitializePredefinedRulesForItem(Context, GetItem());
        }
        #endregion

        #region Lazy fields
        private string[] _keys;
        protected string[] Keys =>
            _keys ?? (_keys = DeviceKey.Split('_'));

        private Customer _customer;
        protected Customer Customer =>
            _customer ?? (_customer = Context.Customers.Single(q => q.Username == UserName));

        private Thing _thing;

        protected Thing GetThing()
        {
            if (_thing != null)
                return _thing;

            var key = ThingNativeKeyModel.FromNativeKey(DeviceKey).ToString();

            _thing = Customer.Things.FirstOrDefault(t => t.NativeKey == key);

            if(_thing == null)
                Log.Error($"Default reporter: failed to fetch thing. Base key: {DeviceKey}, retrieved: {key}.");

            return _thing;
        }
             
        private Item _item;

        protected Item GetItem()
        {
            if (_item != null)
                return _item;

            _item = _item = Customer.Things
                .SelectMany(x => x.Items)
                .FirstOrDefault(t => t.NativeKey == DeviceKey);

            if (_item == null)
                Log.Error($"Default reporter: failed to fetch item. Base key: {DeviceKey}.");

            return _item;
        }

        public KnownDeviceRecognizer _recognizer;
        protected KnownDeviceRecognizer Recognizer { get; }

        #endregion

        protected override void OnDispose(bool disposing)
        {
            Context.SaveChanges();
            if (ShouldDisposeContext)
                Context.Dispose();
        }
    }
}