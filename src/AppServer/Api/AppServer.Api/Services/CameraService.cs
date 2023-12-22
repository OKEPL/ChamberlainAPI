using System;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.DataTransfer;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Content.DataContracts.Camera;
using Chamberlain.Common.Domotica;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.Database.Persistency.Model.Enums;
using Chamberlain.ExternalServices.RabbitMq;
using Common.StaticMethods.StaticMethods;
using Microsoft.EntityFrameworkCore;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Services
{
    using System.Collections.Generic;
    using Chamberlain.Common.Extensions;
    using Contracts.Models.RequestModels.Device.Camera;
    using Contracts.Models.ResponseModels.Device.Camera;

    public class CameraService : ICameraService
    {
        [SetterProperty] public IRtspPortService RtspPortService { get; set; }

        public CameraModel GetCameraByItem(string userName, long itemId)
        {
            using (var context = new Entities())
            {
                var camera = context.Items.First(x =>
                    x.Id == itemId && x.Type == ItemTypes.Camera && x.Thing.State == ThingStates.Active &&
                    x.Thing.Customer.Username.Equals(userName));

                var cameraSettings = XmlSerialization.FromString<CameraSettings>(camera.Settings);
                if (cameraSettings == null)
                    throw new ArgumentNullException("cameraSettings not exist");

                var result = new CameraModel
                {
                    id = camera.Id,
                    name = camera.Thing.GivenName,
                    streamUrl = StaticMethods.GetCameraStreamUrl(camera.Settings, camera.Thing.CustomerId),
                    previewUrl = StaticMethods.GetCameraStreamUrl(camera.Settings, camera.Thing.CustomerId),
                    isCurrentStreamMain = cameraSettings.IsCurrentStreamMain,
                    motionDetectionLevel = cameraSettings.MotionDetectionLevel
                };

                return result;
            }
        }

        public CameraModel GetCameraByThing(string userName, long thingId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).First(x => x.Username.Equals(userName));

                var thing = customer.Things.Where(x => x.State == ThingStates.Active)
                    .Single(t => t.Id == thingId);
                
                var camera = thing.Items.Single(i => i.Type == ItemTypes.Camera);

                var cameraSettings = XmlSerialization.FromString<CameraSettings>(camera.Settings);
                if (cameraSettings == null)
                    throw new ArgumentNullException("cameraSettings not exist");

                var result = new CameraModel
                {
                    id = camera.Id,
                    name = camera.Thing.GivenName,
                    streamUrl = StaticMethods.GetCameraStreamUrl(camera.Settings, thing.CustomerId),
                    previewUrl = StaticMethods.GetCameraStreamUrl(camera.Settings, thing.CustomerId),
                    isCurrentStreamMain = cameraSettings.IsCurrentStreamMain,
                    motionDetectionLevel = cameraSettings.MotionDetectionLevel
                };

                return result;
            }
        }

        public List<CameraSettingsModel> GetCamerasExt(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).First(x => x.Username.Equals(userName));

                var things = customer.Things.Where(t => t.State == ThingStates.Active && t.Items.Any(i =>
                        i.Type == ItemTypes.Camera || i.Type == ItemTypes.HubCamera ||
                        i.Type == ItemTypes.VideoIntercomCamera));

                var result = new List<CameraSettingsModel>(from thing in things
                    let item =
                        thing.Items.FirstOrDefault(i =>
                            i.Type == ItemTypes.Camera || i.Type == ItemTypes.HubCamera ||
                            i.Type == ItemTypes.VideoIntercomCamera)
                    let sett = XmlSerialization.FromString<CameraSettings>(item.Settings)
                    where item != null && sett != null
                    select new CameraSettingsModel
                    {
                        ItemId = item.Id,
                        Name = item.Thing.GivenName,
                        MainStreamVideoPath = sett.MainStreamVideoPath,
                        SubStreamVideoPath = sett.SubStreamVideoPath,
                        ImagePath = sett.ImagePath,
                        Login = sett.Login,
                        Password = sett.Password,
                        IsCurrentStreamMain = sett.IsCurrentStreamMain,
                        MotionDetectionLevel = sett.MotionDetectionLevel,
                        HostAddress = sett.HostAddress,
                        HttpPort = sett.HttpPort,
                        RtspPort = sett.RtspPort,
                        BrandName = thing.BrandId,
                        ModelName = thing.ModelId
                    }).ToList();

                return result;
            }
        }

        public CameraSettingsModel GetCameraByItemExt(string userName, long itemId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items)
                    .First(x => x.Username.Equals(userName));

                var thing = customer.Things.Single(t => t.State == ThingStates.Active && t.Items.Any(i => i.Type == ItemTypes.Camera && i.Id == itemId));
                
                var camera = thing.Items.Single(i => i.Id == itemId);

                var cameraSettings = XmlSerialization.FromString<CameraSettings>(camera.Settings);
                if (cameraSettings == null)
                    throw new ArgumentNullException("cameraSettings not exist");

                var result = new CameraSettingsModel
                {
                    ItemId = camera.Id,
                    Name = camera.Thing.GivenName,
                    MainStreamVideoPath = cameraSettings.MainStreamVideoPath,
                    SubStreamVideoPath = cameraSettings.SubStreamVideoPath,
                    ImagePath = cameraSettings.ImagePath,
                    Login = cameraSettings.Login,
                    Password = cameraSettings.Password,
                    IsCurrentStreamMain = cameraSettings.IsCurrentStreamMain,
                    MotionDetectionLevel = cameraSettings.MotionDetectionLevel,
                    HostAddress = cameraSettings.HostAddress,
                    HttpPort = cameraSettings.HttpPort,
                    RtspPort = cameraSettings.RtspPort,
                    BrandName = thing.BrandId,
                    ModelName = thing.ModelId
                };

                return result;
            }
        }
        
        public void UpdateCamera(string userName, CameraSettingsModel cameraSettingsModel)
        {
            //TODO: check usages and delete method
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).First(x => x.Username.Equals(userName));

                var cameraItem = customer.Things.Where(x => x.State == ThingStates.Active).SelectMany(x => x.Items)
                    .Single(x => x.Id == cameraSettingsModel.ItemId &&
                                          (x.Type == ItemTypes.Camera || x.Type == ItemTypes.HubCamera ||
                                           x.Type == ItemTypes.VideoIntercomCamera));

                var cameraSettings = XmlSerialization.FromString<CameraSettings>(cameraItem.Settings, true);
                if (cameraSettings == null)
                    throw new ArgumentNullException("cameraSettings not exist");

                cameraItem.Settings = GetCameraSettings(cameraSettings, cameraSettingsModel);
                cameraItem.CustomName = cameraSettingsModel.Name;

                var cameraThing = cameraItem.Thing;
                cameraThing.BrandId = cameraSettingsModel.BrandName;
                cameraThing.ModelId = cameraSettingsModel.ModelName;
                cameraThing.GivenName = cameraSettingsModel.Name;

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to update camera. Unknown error. {ex.GetDetails()}");
                }

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.StartHandlingItemMessageType, customer.Id,
                    cameraItem.Id, null, string.Empty));
            }
        }

        public void DeleteCamera(string userName, long thingId)
        {
            Helpers.HelperMethods.DeleteThing(userName, thingId);
        }

        public List<SupportedCameraBrandModel> GetSupportedCameras()
        {
            using (var context = new Entities())
            {
                var knownCameras = context.KnownDevices.Where(w => w.DeviceType == ItemTypes.Camera);
                var supportedCameras = new List<SupportedCameraBrandModel>();

                foreach (var brand in knownCameras.Select(x => x.BrandName).Distinct())
                {
                    var supportedBrand = new SupportedCameraBrandModel(brand);
                    foreach (var model in knownCameras.Where(x => x.BrandName == brand))

                        supportedBrand.CameraModels.Add(new SupportedCameraModel
                        {
                            Model = model.ModelName,
                            Parameters = XmlSerialization
                                .FromString<CameraNeededParameters>(model.NeededParameters, true)?
                                .Parameters
                        });

                    supportedCameras.Add(supportedBrand);
                }

                return supportedCameras;
            }
        }

        public CheckResultModel TestHostPort(int port, string ip)
        {
            return RtspPortService.CheckHostPort(ip, port);
        }

        public CheckResultModel TestRtspPort(int port, string ip)
        {
            return RtspPortService.CheckRtspPort(ip, port);
        }

        private static string GetCameraSettings(CameraSettings cameraSettings, CameraSettingsModel cameraSettingsModel)
        {
            cameraSettings.MainStreamVideoPath = cameraSettingsModel.MainStreamVideoPath;
            cameraSettings.SubStreamVideoPath = cameraSettingsModel.SubStreamVideoPath;
            cameraSettings.ImagePath = cameraSettingsModel.ImagePath;
            cameraSettings.Login = cameraSettingsModel.Login;
            cameraSettings.Password = cameraSettingsModel.Password;
            cameraSettings.IsCurrentStreamMain = cameraSettingsModel.IsCurrentStreamMain;
            cameraSettings.HostAddress = cameraSettingsModel.HostAddress;
            cameraSettings.HttpPort = cameraSettingsModel.HttpPort;
            cameraSettings.RtspPort = cameraSettingsModel.RtspPort;
            cameraSettings.MotionDetectionLevel = cameraSettingsModel.MotionDetectionLevel;
            return XmlSerialization.ToString(cameraSettings);
        }
    }
}