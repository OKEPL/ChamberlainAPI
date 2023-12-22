using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule
{
    public class ListRuleNotifiersModel : BaseChamberlainModel
    {
        [Required]
        public List<RuleNotifierModel> NotifiersList { get; set; }
    }
}