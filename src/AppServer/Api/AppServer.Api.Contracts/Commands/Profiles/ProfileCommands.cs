using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Profile;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Profile;

namespace Chamberlain.AppServer.Api.Contracts.Commands.Profiles
{
    public class GetProfiles : HasUserName
    {
        public GetProfiles(string userName) : base(userName) { }
    }

    public class GetProfile : HasUserName
    {
        public GetProfile(string userName, long profileId) : base(userName)
        {
            ProfileId = profileId;
        }

        public long ProfileId { get; }
    }

    public class AddProfile : HasUserName
    {
        public AddProfile(string userName, ProfileModel profileModel) : base(userName)
        {
            ProfileModel = profileModel;
        }

        public ProfileModel ProfileModel { get; }
    }

    public class DeleteProfile : HasUserName
    {
        public long ProfileId { get; }

        public DeleteProfile(string userName, long profileId) : base(userName)
        {
            ProfileId = profileId;
        }
    }

    public class UpdateProfile : HasUserName
    {
        public UpdateProfile(string userName, ProfileModel profileModel) : base(userName)
        {
            ProfileModel = profileModel;
        }

        public ProfileModel ProfileModel { get; }
    }

    public class AddPhoto : HasUserName
    {
        public AddPhoto(string userName, long profileId, FaceViewType modelType) : base(userName)
        {
            ProfileId = profileId;
            ModelType = modelType;
        }

        public long ProfileId { get; }
        public FaceViewType ModelType { get; }
    }
    public class UploadSamples : HasUserName
    {
        public UploadSamples(string userName) : base(userName)
        {}
    }

    public class CheckFaceViewReadiness : HasUserName
    {
        public CheckFaceViewReadiness(string userName, long profileId, FaceViewType modelType) : base(userName)
        {
            ProfileId = profileId;
            ModelType = modelType;
        }

        public long ProfileId { get; }
        public FaceViewType ModelType { get; }
    }
    
    public class StartModelTraining : HasUserName
    {
        public StartModelTraining(string userName, long profileId) : base(userName)
        {
            ProfileId = profileId;
        }

        public long ProfileId { get; }
    }
}