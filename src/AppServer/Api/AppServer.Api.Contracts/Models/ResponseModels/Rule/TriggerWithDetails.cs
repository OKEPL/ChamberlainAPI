using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule
{
    public class TriggerWithDetails
    {
        public TriggerWithDetails()
        {
            AvailableTriggers = new List<DeviceItem>();
        }

        public List<DeviceItem> AvailableTriggers { get; set; }
        public DeviceItemDetails Details { get; set; }
        public RuleTriggerModel Trigger { get; set; }
    }
}
