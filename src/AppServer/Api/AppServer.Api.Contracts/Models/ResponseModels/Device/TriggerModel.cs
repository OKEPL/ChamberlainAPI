using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Chamberlain.Common.Content.DataContracts.ZWave;
using Chamberlain.Common.Contracts.Enums;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device
{
    public class TriggerModel : BaseChamberlainModel
    {
        public TriggerControlRepresentation control_representation { get; set; }

        public string icon { get; set; }

        [Required]
        public long item_id { get; set; }

        public string item_name { get; set; }

        public List<string> possibleValues { get; set; } = new List<string>();

        [Required]
        public string trigger_type { get; set; }

        public int ui_type { get; set; }

        public IoTItemValueRange value_range { get; set; }

        public string value_units { get; set; }
    }
}