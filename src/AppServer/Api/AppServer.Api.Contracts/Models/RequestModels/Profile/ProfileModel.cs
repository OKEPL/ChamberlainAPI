using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Profile;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Profile
{
    public class ProfileModel
    {
        public long ProfileId { get; set; }

        [Required]
        public string Name { get; set; }

        public List<FaceViewReadyModel> FaceViewsReady { get; set; }
    }
}
