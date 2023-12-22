namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device
{
    #region

    using System;
    using System.Collections.Generic;

    using Chamberlain.Common.Content.DataContracts.ZWave;

    #endregion

    public class NewItemModel : BaseChamberlainModel
    {
        public NewItemModel()
        {
            this.ValueRange = new IoTItemValueRange();
        }

        public int category { get; set; }

        public string custom_name { get; set; }

        public string icon { get; set; }

        public long item_id { get; set; }     

        public DateTime last_seen { get; set; }

        public List<string> list_value_type_items { get; set; } 

        public string native_name { get; set; }

        public int position { get; set; }

        public bool read_only { get; set; }

        public string type { get; set; }

        public string units { get; set; }

        public int UXControlType { get; set; }

        public string value { get; set; }          

        public IoTItemValueRange ValueRange { get; set; }
    }
}