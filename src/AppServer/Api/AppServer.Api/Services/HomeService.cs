using Akka.Actor;
using Chamberlain.Common.CameraUriResolver;
using Chamberlain.Common.Content.Commands;
using Chamberlain.Common.Contracts.Enums;
using Chamberlain.Common.Extensions;
using Chamberlain.Common.Settings;
using Chamberlain.Database.Persistency.Model.Enums;
using Chamberlain.Plugins.ImageAcquire.Contracts.Commands;
using Common.StaticMethods.StaticMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Chamberlain.AppServer.Api.Services
{
    using Common.Content.Constants;
    using Common.Content.DataContracts.Camera;
    using Common.Domotica;
    using Contracts.Models.RequestModels.Mode;
    using Contracts.Models.ResponseModels.Device.Camera;
    using Contracts.Models.ResponseModels.Event;
    using Contracts.Models.ResponseModels.Status;
    using Contracts.Services;
    using Database.Persistency.Model;
    using Devices.Interfaces;
    using Newtonsoft.Json;
    using Serilog;
    using StructureMap.Attributes;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using PersistencyItem = Database.Persistency.Model.Item;

    public class HomeService : IHomeService
    {
        [SetterProperty] public IDeviceManager DeviceManager { get; set; }

        public List<EventModel> GetNewestEventsFromId(string userName, int lastSeen, string language)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Events).First(x => x.Username.Equals(userName));

                var events = customer.Events.Where(e => e.Id > lastSeen).OrderByDescending(e => e.DateTime).ToList();
                var now = DateTime.UtcNow;
                var timezone = TimeZoneInfoExtended.FindSystemTimeZoneById(customer.Timezone);
                var resultList =
                    events.Select(e => new EventModel
                    {
                        id = e.Id,
                        itemName = e.ItemId.HasValue ? e.Item.Thing.GivenName : "",
                        code = e.Code,
                        type = e.Type,
                        date = e.DateTime.Add(timezone.GetUtcOffset(DateTime.UtcNow)),
                        secondsAgo = (long)now.Subtract(e.DateTime).TotalSeconds,
                        payload = GetMessageFromPayload(language, e.Type, e.Payload,
                            e.DateTime.Add(timezone.GetUtcOffset(DateTime.UtcNow)))
                    }).ToList();

                return resultList;
            }
        }

        public StatusModel GetStatus(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
            
                var result = new StatusModel
                {
                    mode = new ModeModel(),
                    status = "EVERYTHING'S OK",
                    statusCode = "OK",
                    cameraMovement = 0,
                    nextSchedChange = new NextModeModel
                    {
                        isScheduled = false,
                        mode = new ModeModel()
                    },
                    modeBySchedule = new ModeModel()
                };

                var now = StaticMethods.ConvertToCustomerTimeZone(DateTime.UtcNow, customer.Timezone);
            
                var currentMode = StaticMethods.GetCurrentMode(userName, now);
                if (currentMode != null)
                {
                    result.mode.ModeId = currentMode.Id;
                    result.mode.Name = currentMode.Name;
                }

                var currentMode2 = StaticMethods.GetCurrentModeBySchedule(userName, now);
                if (currentMode2 != null)
                {
                    result.modeBySchedule.ModeId = currentMode2.Id;
                    result.modeBySchedule.Name = currentMode2.Name;
                }
            
                var nextEntry = StaticMethods.GetNextMode(userName, now, context);
                if (nextEntry != null)
                {
                    result.nextSchedChange.isScheduled = true;
                    result.nextSchedChange.mode = new ModeModel
                    {
                        ModeId = nextEntry.Mode.Id,
                        Name = nextEntry.Mode.Name,
                    };

                    if ((int)now.DayOfWeek < nextEntry.Weekday)
                        now = now.AddDays(nextEntry.Weekday - (int)now.DayOfWeek);
                    else if ((int)now.DayOfWeek > nextEntry.Weekday)
                        now = now.AddDays(nextEntry.Weekday + 7 - (int)now.DayOfWeek);

                    result.nextSchedChange.time = new DateTime(now.Year, now.Month, now.Day, nextEntry.StartTime.Hours, nextEntry.StartTime.Minutes, nextEntry.StartTime.Seconds);
                }
            
                var thing = customer.Things
                    .Where(x => x.State == ThingStates.Active)
                    .SelectMany(t => t.Items)
                    .FirstOrDefault(i => (i.Type == ItemTypes.Camera || i.Type == ItemTypes.HubCamera || i.Type == ItemTypes.VideoIntercomCamera) && i.IsMovement); //TODO check
                if (thing != null)
                {
                    result.status = "MOTION DETECTED!";
                    result.statusCode = "MOTION";
                    result.cameraMovement = thing.Id;
                }
                else if (customer.ActiveEvents > 0)
                {
                    result.status = $"{customer.ActiveEvents} NEW EVENT{(customer.ActiveEvents > 1 ? "S" : "")}!";
                    result.statusCode = "EVENTS";
                }

                return result;
            }
        }

        public void ClearEvents(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.FirstOrDefault(x => x.Username.Equals(userName));
                if (customer == null) return;

                customer.ActiveEvents = 0;
                context.SaveChanges();
            }
        }

        public List<EventModel> GetEventsFromBegining(string userName, int number, string language)
        {
            return GetEvents(userName, number, 0, language);
        }

        public List<EventModel> GetEventsFromId(string userName, int number, int lastSeen, string language)
        {
            Fall();
            return GetEvents(userName, number, lastSeen, language);
        }

        public List<CameraModel> GetCamerasWithImages(string userName)
        {
            Customer customer;
            using (var context = new Entities())
            {
                customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).First(x => x.Username.Equals(userName));

                var cameras = customer.Things.Where(x => x.State == ThingStates.Active).SelectMany(thing => thing.Items).ToList();
                var cameraBag = new ConcurrentBag<CameraModel>();

                var hublessCameras = cameras.Where(item => item.Type == ItemTypes.Camera && item.NativeName == MockSpecialItemType.Settings.ToString()).ToList();
                AddCameras(hublessCameras, cameraBag, (item) => StaticMethods.GetCameraStreamUrl(item.Settings, customer.Id));

                var hubCameras = cameras.Where(item => (item.Type == ItemTypes.HubCamera || item.Type == ItemTypes.VideoIntercomCamera || item.Type == ItemTypes.KinectDepthCamera) && item.NativeName == MockSpecialItemType.Settings.ToString()).ToList();
                AddCameras(hubCameras, cameraBag, (item) => StaticMethods.GetCameraStreamUrl(item.Settings, customer.Id, item.NativeKey));

                var resultList = cameraBag.OrderBy(x => x.id).ToList();
                return resultList;
            }
        }

        public CameraImageModel GetCameraImageByThingId(string userName, long thingId)
        {
            PersistencyItem item;
            using (var context = new Entities())
            {
                item = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items)
                    .First(x => x.Username.Equals(userName)).Things.First(x => x.State == ThingStates.Active && x .Id == thingId).Items
                    .First(i =>
                        (i.Type == ItemTypes.Camera ||
                         i.Type == ItemTypes.HubCamera ||
                         i.Type == ItemTypes.VideoIntercomCamera)
                        && i.NativeName == MockSpecialItemType.Settings.ToString());
            }

            string base64Image;
            try
            {
                base64Image = item.Type == ItemTypes.Camera
                    ? GetCameraImageFromCamera(item)
                    : GetCameraImageFromHubCamera(item);

                if (base64Image == null)
                {
                    throw new ArgumentNullException();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("Could not deliver camera image. Camera may be offline.", ex);
            }

            return new CameraImageModel
            {
                id = thingId,
                image = base64Image,
            };
        }

        private static string GetMessageFromPayload(string language, string messageType, string payload, DateTime date)
        {
            try
            {
                Enum.TryParse(messageType, out MessageType messageTypeEnum);
                using (var context = new Entities())
                {
                    var message = context.Messages
                        .Where(x => x.Language.Equals(language))
                        .FirstOrDefault(x => x.MessageId == (int)messageTypeEnum && x.NotifierType == 0);
                    string messageText;
                    if (message != null)
                    {
                        var messageJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(payload);
                        messageText = message.MessagePattern ?? string.Empty;
                        messageText = messageJson.Keys.Aggregate(messageText, (current, key) => current.Replace("{" + key + "}", messageJson[key]));
                    }
                    else
                    {
                        messageText = payload;
                    }

                    messageText = messageText.Replace("{date}", date.ToString("T"));
                    return messageText;
                }
            }
            catch (Exception e)
            {
                Log.Error($"GetMessageFromPayload WEB API method failed: lang:{language} type:{messageType} payload:{payload} date:{date:yyyy-MM-dd HH:mm:ss}\n{e.GetDetails()}");
                return payload;
            }
        }

        private static List<EventModel> GetEvents(string userName, int number, int lastSeen, string language)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
            

                var events = lastSeen == 0 ? customer.Events.OrderByDescending(e => e.DateTime).Take(number).ToList()
                    : customer.Events.Where(e => e.Id < lastSeen).OrderByDescending(e => e.DateTime).Take(number).ToList();
                var now = DateTime.UtcNow;
                var timezone = TimeZoneInfoExtended.FindSystemTimeZoneById(customer.Timezone);
                var resultList = events.Select(e => new EventModel
                {
                    id = e.Id,
                    itemName = e.ItemId.HasValue ? e.Item.Thing.GivenName : "",
                    code = e.Code,
                    type = e.Type,
                    date = e.DateTime.Add(timezone.GetUtcOffset(DateTime.UtcNow)),
                    secondsAgo = (long)now.Subtract(e.DateTime).TotalSeconds,
                    payload = GetMessageFromPayload(language, e.Type, e.Payload, e.DateTime.Add(timezone.GetUtcOffset(DateTime.UtcNow)))
                }).ToList();

                return resultList;
            }
        }

        private static string DetermineCameraStatus(DateTime lastSeen)
        {
            var now = DateTime.UtcNow;
            return (now - lastSeen).TotalMinutes > 10 ? "offline" : "online";
        }

        private void AddCameras(IEnumerable<PersistencyItem> cameras, ConcurrentBag<CameraModel> cameraBag,
                                                                            Func<PersistencyItem, string> streamUrl)
        {
            Parallel.ForEach(cameras.ToList(), async (item) =>
            {
                cameraBag.Add(new CameraModel
                {
                    id = item.ThingId,
                    itemId = item.Id,
                    name = item.Thing.GivenName,
                    streamUrl = streamUrl(item),
                    previewUrl = streamUrl(item),
                    image =  item.Type == ItemTypes.Camera ? await GetImage(CameraUriResolver.GetUri(item.Id, UriType.Image)) : string.Empty,
                    status = DetermineCameraStatus(item.LastSeen ?? DateTime.MinValue),
                    isCurrentStreamMain = XmlSerialization.FromString<CameraSettings>(item.Settings).IsCurrentStreamMain,
                    motionDetectionLevel = XmlSerialization.FromString<CameraSettings>(item.Settings).MotionDetectionLevel
                });
            });
        }

        private string GetCameraImageFromHubCamera(PersistencyItem item)
        {
            //TODO: this solution is temporary and should be replaced after hub-tasks logic implementation
            ThumbnailData thData = null;            
            try
            {
                var hubCameraActor = DeviceManager.Device(d => d.ItemId == item.Id).GetActorRef();
                thData = hubCameraActor.Ask<ThumbnailData>(new GetThumbnail(item.NativeKey), TimeSpan.FromMilliseconds(CachedSettings.Get<int>("CameraPreviewImageTimeoutMilliseconds", 10000))).Result;                
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error getting image for hub camera {item.NativeKey}");
                return string.Empty;
            }

            using (var ms = new MemoryStream())
            {
                using (var thumbnail = thData.Thumbnail)
                {
                    thumbnail.Save(ms, ImageFormat.Jpeg);
                    return $"data:image/jpeg;base64, {Convert.ToBase64String(ms.ToArray())}";
                }
            }
        }

        private string GetCameraImageFromCamera(PersistencyItem item)
        {
            var imageUrl = CameraUriResolver.GetUri(item.Id, UriType.Image);

            for (var i = 1; i <= 3; i++)
            {
                var image = GetImage(imageUrl, CachedSettings.Get("CameraPreviewImageTimeoutMilliseconds", 5000)).Result;
                // TODO: Replace with settings after removing image download in GetCameras() method.
                if (!string.IsNullOrEmpty(image))
                    return image;
            }

            return string.Empty;
        }

        private async Task<string> GetImage(string url)
        {
            return await GetImage(url, CachedSettings.Get<int>("CameraPreviewImageTimeoutMilliseconds", 5000));
        }

        private async Task<string> GetImage(string url, int timeout)
        {
            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(timeout) })
                {
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Log.Error($"Error during downloading camera stream in HomeController. Address: {url}.");
                        return string.Empty;
                    }

                    var img = Image.FromStream(await response.Content.ReadAsStreamAsync());
                    var width = CachedSettings.Get<int>("CameraPreviewImageWidth", 340);
                    var height = (int)((width * 1.0) * 9 / 16);
                    img = StaticMethods.ResizeImage(img, new Size(width, height));
                    using (var ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        var imageBytes = ms.ToArray();
                        return "data:image/png;base64, " + Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                Log.Debug($"Error during checking camera address: {url}. Details: {e.GetDetails()}");
            }

            return string.Empty;
        }

        private void Fall()
        {
            var person = CachedSettings.Get("Person", "6");
            using (var context = new Entities())
            {
                var customer = context.Customers.First(c => c.Id == 1);
                if (customer.CurrentModeId != 60)
                {
                    return;
                }

                var item = context.Items.First(i => i.Id == 1915);
                item.NativeValue = person;
                context.SaveChanges();

                var hubCameraActor = DeviceManager.Device(d => d.ItemId == 1887).GetActorRef();
                var valueChangedMsg = new ItemValueChanged(1915, person);
                hubCameraActor.Tell(valueChangedMsg);
            }
        }
    }
}