using System.Collections.Generic;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule
{
    public class TriggerType : BaseChamberlainModel
    {
        public TriggerType()
        {
            Options = new List<DeviceBasicInfo>();
        }

        public string Name { get; set; }
        public List<DeviceBasicInfo> Options { get; set; }
        public string RuleTriggerType { get; set; }
    }
}