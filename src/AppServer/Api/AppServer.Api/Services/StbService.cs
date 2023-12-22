using System;
using System.Linq;

using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Domotica;
using Chamberlain.Common.Settings;
using Chamberlain.Database.Persistency.Model;
using ExternalServices.Stb;
using Microsoft.EntityFrameworkCore;
using PredefinedRulesManager.Interfaces;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Services
{
    public class StbService : IStbService
    {
        [SetterProperty] public IPredefinedRulesManagerPlugin PredefinedRulesManagerPlugin { get; set; }

        private const string deviceType = "stb";
        private const string deviceName = "Set-Top Box";

        public void StbLogin(string userName, string solocooLogin, IActorRef actorRef)
        {
            var solocooClient = new SolocooClient(solocooLogin); 
            var stbDevice = solocooClient.GetDevices().FirstOrDefault(x => x.DeviceType == deviceType);

            if (stbDevice != null)
            {
                using (var context = new Entities())
                {
                    var customer = context.Customers.First(x => x.Username.Equals(userName));

                    var knownThing = context.KnownDevices
                                        .Where(x => x.BrandName == deviceType && x.ModelName == deviceType)
                                        .Include(i => i.KnownItems)
                                        .FirstOrDefault();

                    var knownItem = knownThing?.KnownItems?.Where(x => x.Name == deviceType).FirstOrDefault();

                    var stbThing = new Thing
                    {
                        BrandId = deviceType,
                        ModelId = deviceType,
                        CustomerId = customer.Id,
                        GivenName = deviceName,
                        NativeKey = $"{deviceType}_{stbDevice.SerialNumber}",
                        KnownDevice = knownThing
                    };
                    context.Things.Add(stbThing);

                    var stbItem = new Item
                    {
                        CustomName = deviceType,
                        NativeValue = deviceType,
                        IsMovement = false,
                        Settings = XmlSerialization.ToString(new StbItemSettings
                        {
                            SolocooUser = solocooLogin,
                            Serial = stbDevice.SerialNumber
                        }),
                        Type = deviceType,
                        NativeKey = $"{deviceType}_{stbDevice.SerialNumber}",
                        NativeName = deviceType,
                        Thing = stbThing,
                        KnownItem = knownItem
                    };
                    context.Items.Add(stbItem);
                    context.SaveChanges();
                    PredefinedRulesManagerPlugin.InitializePredefinedRulesForItem(context, stbItem);
                }
            }
        }
    }
}