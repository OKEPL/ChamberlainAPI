using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule
{
    public class ActionWithDetails
    {
        public ActionWithDetails()
        {
            AvailableActions = new List<DeviceItem>();
        }

        public List<DeviceItem> AvailableActions { get; set; }
        public DeviceItemDetails Details { get; set; }
        public RuleActionModel Action { get; set; }
    }
}
