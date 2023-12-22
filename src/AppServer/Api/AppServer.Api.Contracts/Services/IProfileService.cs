using System.Collections.Generic;
using Akka.Actor;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Profile;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Profile;

namespace Chamberlain.AppServer.Api.Contracts.Services
{
    public interface IProfileService
    {
        SamplesModel UploadSamples(string userName);
        List<ProfileModel> GetProfiles(string userName);    
        ProfileModel GetProfile(string userName, long ruleId);
        ProfileModel AddProfile(string userName, ProfileModel profileModel);
        ProfileModel UpdateProfile(string userName, ProfileModel profileModel);
        PhotoModel AddImage(string userName, long profileId, FaceViewType viewType);
        FaceViewReadyModel CheckFaceViewReadiness(string userName, long profileId, FaceViewType viewType);
        void DeleteProfile(string userName, long profileId);
        void StartModelTraining(string userName, long profileId, IActorRef overseerActor);
        
    }
}
