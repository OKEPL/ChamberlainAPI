using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule
{
    public class RuleModeInfoModel : RuleModeModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
