using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Util.Internal;
using Chamberlain.AppServer.Api.Contracts.Commands.Profiles;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Profile;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Profile;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.Common.Extensions;
using Chamberlain.Common.Settings;
using Chamberlain.Common.VideoInterface;
using Chamberlain.Common.VideoInterface.FaceData;
using Chamberlain.Plugins.FaceRecognitionTrainer.Contracts.Commands;
using Common.StaticMethods.StaticMethods;
using Microsoft.EntityFrameworkCore;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Services
{
    public class ProfileService : IProfileService
    {
        private const int MinimalLabel = 1001;

        [SetterProperty] public IHubNotifierService HubNotifierService { get; set; }

        public List<ProfileModel> GetProfiles(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.FaceProfiles).First(x => x.Username.Equals(userName));
                var profileModelsList = customer.FaceProfiles.Select(FaceProfileToProfileModel).ToList();

                return profileModelsList;
            }
        }

        public ProfileModel GetProfile(string userName, long profileId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.FaceProfiles).First(x => x.Username.Equals(userName));
                var profile = customer.FaceProfiles.First(x => x.Id == profileId);

                return FaceProfileToProfileModel(profile);
            }
        }

        public ProfileModel AddProfile(string userName, ProfileModel profileModel)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.FaceProfiles).First(x => x.Username.Equals(userName));
                var faceProfile = new FaceProfile { Name = profileModel.Name };
                customer.FaceProfiles.Add(faceProfile);
                context.SaveChanges();
                HubNotifierService.Notify(new PossibleGrammarUpdateNotification { UserName = userName });

                return FaceProfileToProfileModel(faceProfile);
            }
        }

        public ProfileModel UpdateProfile(string userName, ProfileModel profileModel)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.FaceProfiles).First(x => x.Username.Equals(userName));
                var faceProfile = customer.FaceProfiles.First(x => x.Id == profileModel.ProfileId);
                faceProfile.Name = profileModel.Name;
                context.SaveChanges();
                HubNotifierService.Notify(new PossibleGrammarUpdateNotification { UserName = userName });

                return FaceProfileToProfileModel(faceProfile);
            }
        }

        public void DeleteProfile(string userName, long profileId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.FaceProfiles).First(x => x.Username.Equals(userName));
                var profile = customer.FaceProfiles.First(x => x.Id == profileId);

                var profileViewsLabels = profile.FaceViews.Select(x => x.Label).ToList();
                var faceSamplesData = new FaceSamplesDataFile();
                var photosNamesToDelete = faceSamplesData.FindAndRemovePhotosOfLabels(profileViewsLabels, customer.Id);
                DeleteImages(photosNamesToDelete, customer.Id);
                DeleteProfilePhoneNumber(profileId, context);

                context.FaceProfiles.Remove(profile);
                context.SaveChanges();
            }

            HubNotifierService.Notify(new PossibleGrammarUpdateNotification { UserName = userName });
        }

        public void StartModelTraining(string userName, long profileId, IActorRef overseerActor)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.FaceProfiles).First(x => x.Username.Equals(userName));
                overseerActor.Tell(new SetUpTraining(GetCustomersFaceSamples(customer, profileId), customer.Id));
            }
        }

        public PhotoModel AddImage(string userName, long profileId, FaceViewType viewType)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.FaceProfiles).ThenInclude(x => x.FaceViews).First(x => x.Username.Equals(userName));
                var profile = customer.FaceProfiles.First(p => p.Id == profileId);
                var view = profile.FaceViews.FirstOrDefault(v => v.Type == viewType.ToDescription());
                if (view == null)
                {
                    view = profile.FaceViews.FirstOrDefault(v => v.Type == viewType.ToDescription());
                    if (view == null)
                    {
                        var faceViews = customer.FaceProfiles.SelectMany(p => p.FaceViews).ToList();

                        view = new FaceView
                        {
                            Type = viewType.ToDescription(),
                            Label = GetNewFaceViewLabel(faceViews)
                        };

                        profile.FaceViews.Add(view);
                        context.SaveChanges();
                    }
                }

                var photoFileName = Guid.NewGuid().ToString("N") + ".jpg";
                var photoPath = FaceRecognitionOverseerDbSettings.RawSamplesCloudPath(customer.Id);
                StaticMethods.CreateDirectory(photoPath);
                var faceSamplesData = new FaceSamplesDataFile();
                faceSamplesData.AppendPhotoInformation(photoFileName, view.Label, customer.Id);

                return new PhotoModel
                {
                    CustomerId = customer.Id,
                    ModelLabel = view.Label,
                    ProfileId = profile.Id,
                    FileName = photoFileName
                };
            }
        }

        public SamplesModel UploadSamples(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                return new SamplesModel
                {
                    CustomerId = customer.Id
                };
            }
        }

        public FaceViewReadyModel CheckFaceViewReadiness(string userName, long profileId, FaceViewType viewType)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.FaceProfiles).First(x => x.Username.Equals(userName));
                var profile = customer.FaceProfiles.First(p => p.Id == profileId);

                return FaceSamplesDataFile.CheckReadinessOfFaceView(profile, viewType, customer.Id);
            }
        }

        private static void DeleteProfilePhoneNumber(long profileId, Entities context)
        {
             context.PhoneNumbers.Where(x => x.FaceProfileId == profileId).ForEach(x=> x.FaceProfileId = null);
        }

        private static int GetNewFaceViewLabel(IReadOnlyCollection<FaceView> faceViews)
        {
            return faceViews.Any() && faceViews.Max(v => v.Label) >= MinimalLabel 
                        ? faceViews.Max(v => v.Label) + 1 
                        : MinimalLabel;
        }

        private static ProfileModel FaceProfileToProfileModel(FaceProfile profile)
        {
            return new ProfileModel
            {
                Name = profile.Name,
                ProfileId = profile.Id,
                FaceViewsReady = FaceSamplesDataFile.CheckReadinessOfFaceViews(profile)
            };
        }

        private static void DeleteImages(IEnumerable<string> photosNames, long customerId)
        {
            foreach (var photoName in photosNames)
            {
                var photoPath = ModelTrainingPhotosPaths.PhotoFilePath(customerId, photoName);
                File.Delete(photoPath);
            }
        }

        private static List<AggregatedData> GetCustomersFaceSamples(Customer user, long profileId)
        {
            var profile = user.FaceProfiles.First(faceProfile => faceProfile.Id == profileId);
            var faceView = profile.FaceViews.Select(view => view.Label).ToList();
            var samplesFilePath = FaceRecognitionOverseerDbSettings.SamplesListFileCloudPath(user.Id);

            using (var fileStream = File.Open(samplesFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                return JsonHelper.LoadPhotosInformations(fileStream).Where(photo => faceView.Contains(int.Parse(photo.FaceView))).ToList();
        }
    }
}