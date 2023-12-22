using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using Akka.Actor;
using Akka.Util.Internal;
using Chamberlain.AppServer.Api.Contracts.Exceptions;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Authentication;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.AppServer.Api.Helpers;
using Chamberlain.Common.Content.Commands;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Content.Interfaces;
using Chamberlain.Common.Content.StructureMapContent;
using Chamberlain.Common.Domotica;
using Chamberlain.Common.Settings;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.Database.Persistency.Model.Enums;
using Chamberlain.ExternalServices.Email;
using Chamberlain.ExternalServices.Google.Maps;
using Chamberlain.ExternalServices.RabbitMq;
using Chamberlain.Plugins.PredefinedModesManager;
using Common.StaticMethods.StaticMethods;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StructureMap.Attributes;
using Address = Chamberlain.Database.Persistency.Model.Address;

namespace Chamberlain.AppServer.Api.Services
{

    public class CustomerService : ICustomerService
    {
        [SetterProperty] public IEmailSender EmailSender { get; set; }
        [SetterProperty] public ICustomerEmailService CustomerEmailService { get; set; }
        [SetterProperty] public INestAgentPlugin NestAgentPlugin { get; set; }
        [SetterProperty] public IGeocodingService GeocodingService { get; set; }
        [SetterProperty] public IPredefinedModesManagerPlugin PredefinedModesManagerPlugin { get; set; }

        public void AddUser(string name, string email, string password, IActorRef ruleEngineActorRef)
        {
            var token = Guid.NewGuid();
            Customer customer;

            using (var tran = new TransactionScope())
            {
                customer = new Customer
                {
                    Username = name,
                    Email = email,
                    Password = StaticMethods.HashPassword(password),
                    Pin = 1234,
                    Language = "nl-NL",
                    Timezone = TimeZoneInfoExtended.DefaultTimeZone,
                    DateJoined = DateTime.UtcNow,
                    IsActive = false,
                    Token = token.ToString()
                };

                using (var context = new Entities())
                {
                    context.Customers.Add(customer);
                    context.SaveChanges();
                    var customerFeature = new CustomerFeatureBinding
                    {
                        CustomerId = customer.Id,
                        FeatureId = CachedSettings.Get("DefaultFeatureId", 1),
                        StartDate = DateTime.UtcNow
                    };
                    context.CustomerFeatureBindings.Add(customerFeature);
                    context.SaveChanges();
                }

                // add registration email to camsafe account
                CustomerEmailService.AddEmail(customer.Username, email, true, false);

                tran.Complete();
            }

            PredefinedModesManagerPlugin.FullyInitializeForCustomer(customer.Id);
            CreateUserDirectories(customer.Id);
            ruleEngineActorRef.Tell(new CustomerAdded(customer.Id));
        }

        public void ChangeTimezone(string userName, int timezone)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                if (customer == null)
                    return;
                customer.Timezone = timezone;
                context.SaveChanges();
                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, ""));
            }
        }

        public UserModel GetUser(string userName)
        {
            using (var context = new Entities())
            {
                //TODO: check usages and change retrieving customer to GetCustomer
                
                var customer = context.Customers.SingleOrDefault(q => q.Username == userName);
                if (customer == null)
                {
                    return null;
                }

                var model = new UserModel
                {
                    Id = customer.Id,
                    UserName = customer.Username,
                    Email = customer.Email,
                    LastLogin = customer.LastLogin ?? DateTime.UtcNow,
                    CurrentMode = customer.CurrentModeId.HasValue
                        ? new ModeModel
                        {
                            ModeId = customer.CurrentModeId.Value,
                            Name = customer.CurrentMode.Name
                        }
                        : new ModeModel
                        {
                            ModeId = 0,
                            Name = "None",
                        },
                    Settings = context.Settings.Select(s => new SettingModel { Id = s.Id, Name = s.Name, Value = s.Value })
                        .ToList()
                };

                return model;
            }
        }

        public void StartRestorePassword(string name)
        {
            using (var context = new Entities())
            {
                var user = context.Customers.First(c => c.Username == name);

                var newToken = Guid.NewGuid();
                user.Token = newToken.ToString();
                var original = context.Customers.Find(user.Id);
                if (original != null)
                {
                    context.Entry(original).CurrentValues.SetValues(user);
                    context.SaveChanges();
                }

                var mailBody = EmailSender.GetEmailTemplate("CustomerRestorePasswordEmail");
                mailBody = mailBody.Replace("{link}", CachedSettings.Get("CustomerPasswordMailLink", "http://localhost/Account/Login?method=restore&amp;Token="));
                mailBody = mailBody.Replace("{token}", user.Token);
                mailBody = mailBody.Replace("{userName}", user.Username);
                EmailSender.Send(user.Email, "Domotica change password email.", mailBody);
            }
        }

        public void RestorePassword(string userName, string password)
        {
            using (var context = new Entities())
            {
                var user = context.Customers.Single(q => q.Username == userName);

                user.Password = StaticMethods.HashPassword(password);
                user.Token = null;
                context.SaveChanges();
            }
        }

        public List<TimeZoneInfoModel> GetTimezones()
        {
            return GetNormalizedTimezones();
        }

        public UserSubscriptionModel GetUserSubscription(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Recordings).Include(x => x.Things).ThenInclude(x => x.Items).Include(x => x.CustomerFeatureBindings).ThenInclude(x => x.Feature)
                    .First(x => x.Username.Equals(userName));

                var feature = customer.CustomerFeatureBindings.FirstOrDefault(fb => fb.StartDate < DateTime.UtcNow && (!fb.EndDate.HasValue || fb.EndDate.Value > DateTime.UtcNow))?.Feature;

                if (feature == null)
                    return new UserSubscriptionModel();

                var details = XmlSerialization.FromString<FeatureDetails>(feature.FeatureDetails);
                var userRecordings = customer.Recordings.ToList();
                var recordingsSpace = userRecordings.Sum(userRecording => userRecording.Size);
                if (recordingsSpace > details.DiskSpace)
                    recordingsSpace = details.DiskSpace;

                var userCameras = customer.Things.Where(x => x.State == ThingStates.Active)
                    .Sum(thing => thing.Items.Count(i => i.Type == ItemTypes.Camera));

                return new UserSubscriptionModel
                {
                    SubscriptionId = feature.Id,
                    SubscriptionName = feature.Name,
                    RecordingsNo = userRecordings.Count,
                    SpaceLeft = details.DiskSpace - recordingsSpace,
                    Space = details.DiskSpace,
                    Cameras = userCameras,
                    CamerasMax = details.Cameras,
                    Hub = details.Hub
                };
            }
        }

        public void ChangeUserSubscription(string userName, long featureId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.CustomerFeatureBindings)
                    .First(x => x.Username.Equals(userName));

                foreach (var customerFeatureBindingse in customer.CustomerFeatureBindings)
                {
                    if (customerFeatureBindingse.EndDate.HasValue &&
                        customerFeatureBindingse.EndDate.Value < DateTime.UtcNow)
                        continue;
                    customerFeatureBindingse.EndDate = DateTime.UtcNow;
                }

                context.CustomerFeatureBindings.Add(new CustomerFeatureBinding
                {
                    CustomerId = customer.Id,
                    FeatureId = featureId,
                    StartDate = DateTime.UtcNow
                });
                context.SaveChanges();
            }
        }

        public void ChangeUserMode(string userName, long modeId, IActorRef ruleEngineActiorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Modes).FirstOrDefault(x => x.Username.Equals(userName));
                if (customer == null)
                {
                    return;
                }

                customer.CurrentModeId = modeId;
                context.SaveChanges();

                var mode = customer.Modes.First(m => m.Id == modeId);
                var message = $"Mode changed to \"{mode.Name}\".";

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSecurityModeChangedMessageType, customer.Id,null, null, message, true));
                ruleEngineActiorRef.Tell(new CustomerRulesChanged(mode.CustomerId));
                ruleEngineActiorRef.Tell(new CustomerModeChanged(customer.Id, mode.Id, mode.NobodyAtHome));
            }
        }

        public AccountDataModel GetAccountData(string userName)
        {
            var res = new AccountDataModel();

            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.PhoneNumbers).Include(x => x.Emails).Include(x => x.IftttKeys).Include(x => x.CustomerFeatureBindings).ThenInclude(x => x.Feature).Include(x => x.Recordings).Include(x => x.Things).ThenInclude(x => x.Items).First(x => x.Username.Equals(userName));

                res.preRecordingTime = customer.RecordingPreMotionDetectionBufferSeconds;
                res.postRecordingTime = customer.RecordingPostMotionDetectionBufferSeconds;

                foreach (var email in customer.Emails)
                {
                    var emailModel = new EmailModel
                    {
                        Email = email.Address,
                        Alerts = email.AlertActive,
                        Newsletters = email.NewsletterActive,
                        Label = email.Address
                    };

                    res.emailList.Add(emailModel);
                }

                foreach (var phone in customer.PhoneNumbers.Where(x => !x.IsSecurity))
                {
                    var securityPhoneModel = new SmsModel
                    {
                        PhoneNumber = phone.Number,
                        Label = phone.Label,
                        ProfileName = phone.FaceProfile?.Name,
                        ProfileId = phone.FaceProfileId,
                        Alerts = phone.SmsActive,
                        Voip = phone.VoipActive
                    };

                    res.smsList.Add(securityPhoneModel);
                }

                foreach (var phone in customer.PhoneNumbers.Where(x => x.IsSecurity))
                {
                    var securityPhoneModel = new SecurityPhoneModel
                    {
                        PhoneNumber = phone.Number,
                        Label = phone.Label,
                        Alerts = phone.SmsActive,
                        Voip = phone.VoipActive
                    };

                    res.securityList.Add(securityPhoneModel);
                }

                foreach (var key in customer.IftttKeys)
                {
                    var iftttModel = new IftttModel
                    {
                        Ifttt = key.Key,
                        Label = key.Label,
                        Alerts = key.AlertActive
                    };

                    res.iftttList.Add(iftttModel);
                }

                res.timezone = customer.Timezone;

                var subscriptionBinding =
                    customer.CustomerFeatureBindings.FirstOrDefault(
                        fb =>
                            fb.StartDate < DateTime.UtcNow &&
                            (!fb.EndDate.HasValue || fb.EndDate.Value > DateTime.UtcNow));

                if (subscriptionBinding == null)
                    return res;

                var details = XmlSerialization.FromString<FeatureDetails>(subscriptionBinding.Feature.FeatureDetails);

                var userRecordings = customer.Recordings.ToList();
                var recordingsSpace = userRecordings.Sum(userRecording => userRecording.Size);
                if (recordingsSpace > details.DiskSpace)
                    recordingsSpace = details.DiskSpace;

                var userCameras = customer.Things.Where(x => x.State == ThingStates.Active)
                    .Sum(t => t.Items.Count(i => i.Type == ItemTypes.Camera));

                res.subscription = new UserSubscriptionModel
                {
                    SubscriptionId = subscriptionBinding.Feature.Id,
                    SubscriptionName = subscriptionBinding.Feature.Name,
                    RecordingsNo = userRecordings.Count,
                    SpaceLeft = details.DiskSpace - recordingsSpace,
                    Space = details.DiskSpace,
                    Cameras = userCameras,
                    CamerasMax = details.Cameras,
                    Sms = details.Sms,
                    Voip = details.Voip,
                    Hub = details.Hub
                };

                Feature feature = null;
                foreach (var feature1 in context.Features)
                {
                    var d = XmlSerialization.FromString<FeatureDetails>(feature1.FeatureDetails);
                    if (d.OrderNo <= details.OrderNo ||
                        feature != null &&
                        d.OrderNo >= XmlSerialization.FromString<FeatureDetails>(feature.FeatureDetails).OrderNo)
                        continue;

                    feature = feature1;
                }

                if (feature == null)
                    return res;

                res.isLastSubscription = false;

                var det = XmlSerialization.FromString<FeatureDetails>(feature.FeatureDetails);
                res.nextSubscription = new UserSubscriptionModel
                {
                    SubscriptionId = feature.Id,
                    SubscriptionName = feature.Name,
                    RecordingsNo = userRecordings.Count,
                    SpaceLeft = det.DiskSpace - recordingsSpace,
                    Space = det.DiskSpace,
                    Cameras = userCameras,
                    CamerasMax = det.Cameras,
                    Sms = det.Sms,
                    Voip = det.Voip,
                    Hub = det.Hub
                };

                var agent = ObjectFactory.Container.GetInstance<INestAgentPlugin>();
                res.nestConnectionStatus = agent.IsCustomerConnectedWithNest(customer.Id);
            }

            return res;
        }

        public void UpdateRecordingBracketsTime(string userName, long preRecTime, long postRecTime)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                customer.RecordingPostMotionDetectionBufferSeconds = postRecTime;
                customer.RecordingPreMotionDetectionBufferSeconds = preRecTime;
                context.SaveChanges();
            }
        }

        public void UpdateNotification(string userName, UpdateAccountNotificationsModel data)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Emails).Include(x => x.PhoneNumbers).Include(x => x.IftttKeys).First(x => x.Username.Equals(userName));

                customer.Emails.Clear();
                customer.PhoneNumbers.Clear();
                customer.IftttKeys.Clear();

                foreach (var notification in data.Notifications)
                {
                    switch (notification)
                    {
                        case EmailModel email:
                            customer.Emails.Add(new Email
                            {
                                Address = email.Email,
                                AlertActive = email.Alerts,
                                NewsletterActive = email.Newsletters,
                                Label = email.Label
                            });
                            continue;
                        case SmsModel sms:
                            customer.PhoneNumbers.Add(new PhoneNumber
                            {
                                Number = sms.PhoneNumber,
                                SmsActive = sms.Alerts,
                                VoipActive = sms.Voip,
                                Label = sms.Label
                            });
                            continue;
                        case IftttModel ifttt:
                            customer.IftttKeys.Add(new IftttKey
                            {
                                Key = ifttt.Ifttt,
                                AlertActive = ifttt.Alerts,
                                Label = ifttt.Label
                            });
                            break;
                    }
                }
                
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null, null, string.Empty));
            }
        }

        public NestAuthenticationSessionModel CreateNestAuthenticationSession(string userName, string redirectTo)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                var sessionHash = HelperMethods.CalculateMD5Hash(DateTime.Now.ToString(CultureInfo.InvariantCulture));

                var session = new NestOauthSession
                {
                    CustomerToken = customer.Username,
                    RedirectUrl = redirectTo,
                    SessionHash = sessionHash,
                    CustomerId = customer.Id
                };

                context.NestOauthSessions.Add(session);
                context.SaveChanges();

                return new NestAuthenticationSessionModel
                {
                    SessionHash = sessionHash
                };
            }
        }

        public NestRedirectionModel GetNestTokenAndRedirectBack(string state, string code)
        {
            var token = HelperMethods.RetrieveNestAccessToken(code);
            using (var context = new Entities())
            {
                var session = context.NestOauthSessions.SingleOrDefault(x => x.SessionHash == state);

                if(session == null)
                    return new NestRedirectionModel();

                NestAgentPlugin.ConnectCustomerWithNest(context, session.CustomerId, token, session.CustomerToken);
                context.NestOauthSessions.Remove(session);
				context.SaveChanges();

                return new NestRedirectionModel { RedirectUri = new Uri(session.RedirectUrl) };
            }
        }

        public void DiscardNestConnection(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).First(x => x.Username.Equals(userName));

                NestAgentPlugin.DisconnectCustomerFromNest(context, customer.Id, customer.Username);

                var nestThings = customer.Things.Where(x => x.BrandId == "Nest").ToList();

                foreach (var thing in nestThings)
                    HelperMethods.DeleteThing(userName, thing.Id);
            }
        }

        public CustomerAddressModel GetAddress(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Addresses).First(x => x.Username.Equals(userName));

                var address = customer.Addresses.FirstOrDefault();

                if (address == null)
                    return new CustomerAddressModel();

                return new CustomerAddressModel
                {
                    City = address.City,
                    Country = address.Country,
                    Street = address.Street,
                    HouseNumber = address.HouseNumber,
                    HouseNumberExtension = address.HouseNumberExtension,
                    PostalCode = address.PostalCode,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude

                };
            }
        }

        public void AssignAddress(string userName, CustomerAddressBaseModel data)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Addresses).First(x => x.Username.Equals(userName));

                var address = customer.Addresses.FirstOrDefault();

                if (address != null)
                {
                    throw new HttpConflictException("Address for this user already exists. Please use update method");
                }
                
                address = new Address
                {
                    CustomerId = customer.Id,
                    Country = data.Country,
                    City = data.City,
                    PostalCode = data.PostalCode,
                    Street = data.Street,
                    HouseNumber = data.HouseNumber,
                    HouseNumberExtension = data.HouseNumberExtension

                };

                var coordinates = GeocodingService.GetCoordinates(data.Country, data.City, data.PostalCode, data.Street, data.HouseNumber);
                if (coordinates.IsSuccess && coordinates.Response != null)
                {
                    address.Latitude = coordinates.Response.Latitude;
                    address.Longitude = coordinates.Response.Longitude;
                }

                context.Addresses.Add(address);
                context.SaveChanges();
            }
        }

        public void UpdateAddress(string userName, CustomerAddressBaseModel data)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Addresses).First(x => x.Username.Equals(userName));

                var address = customer.Addresses.FirstOrDefault();

                if (address == null)
                {
                    AssignAddress(userName, data);
                    return;
                }

                address.Country = data.Country;
                address.City = data.City;
                address.PostalCode = data.PostalCode;
                address.Street = data.Street;
                address.HouseNumber = data.HouseNumber;
                address.HouseNumberExtension = data.HouseNumberExtension;

                var coordinates = GeocodingService.GetCoordinates(data.Country, data.City, data.PostalCode, data.Street,
                    data.HouseNumber);
                if (coordinates.IsSuccess && coordinates.Response != null)
                {
                    address.Latitude = coordinates.Response.Latitude;
                    address.Longitude = coordinates.Response.Longitude;
                }

                context.SaveChanges();
            }
        }

        private static void CreateUserDirectories(long createdUserId)
        {
            var videoPath = CachedSettings.Get("VideoPath", "C:\\CustomerContentStorage\\User_{userId}").Replace("{userId}", createdUserId.ToString());
            var recPath = CachedSettings.Get("RecordingOutputPath", "C:\\CustomerContentStorage\\User_{userId}").Replace("{userId}", createdUserId.ToString());

            CreateDirectory(videoPath);

            CreateDirectory(videoPath + "\\Thumbnails");
            CreateDirectory(videoPath + "\\preview");
            CreateDirectory(videoPath + "\\recording");
            CreateDirectory(recPath + "\\Thumbnails");
            CreateDirectory(recPath + "\\recording");
        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private List<TimeZoneInfoModel> GetNormalizedTimezones()
        {
            var result = TimeZoneInfo.GetSystemTimeZones().Select(t => new TimeZoneInfoModel
            {
                Id = TimeZoneInfoExtended.TranslateTimezoneStringIdToInt(t.Id),
                TimezoneName = t.Id,
                TimezoneType = t.DisplayName
            });

            return NormalizeTimezones(result.ToList());
        }

        private static List<TimeZoneInfoModel> NormalizeTimezones(List<TimeZoneInfoModel> timezones)
        {
            var missing = timezones.Where(w => w.Id == -1).ToList();
            foreach (var timezone in missing)
            {
                Log.Warning($"Could not find timezone id for {timezone.TimezoneName}");
            }

            return timezones.Except(missing).ToList();
        }
    }
}