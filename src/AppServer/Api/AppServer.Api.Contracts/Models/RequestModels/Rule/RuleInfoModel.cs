using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule
{
    public class RuleInfoModel : BaseChamberlainModel
    {
        public RuleInfoModel()
        {
            Notifiers = new List<RuleNotifierModel>();
            Modes = new List<RuleModeInfoModel>();
        }
        public long RuleId { get; set; }
        public string RuleName { get; set; }
        public bool IsPredefined { get; set; }
        public List<RuleNotifierModel> Notifiers { get; set; }
        public List<RuleModeInfoModel> Modes { get; set; }
    }
}
