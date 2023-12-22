using System.ComponentModel;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Profile
{
    public enum FaceViewType
    {
        [Description("frontal")]
        Frontal,

        [Description("frontalFromAbove")]
        FrontalFromAbove,

        [Description("leftProfile")]
        LeftProfile,

        [Description("rightProfile")]
        RightProfile
    }
}
