using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Scene;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device
{
    using System.Collections.Generic;

    /// <summary>
    ///     Thing representation
    /// </summary>
    public class DeviceModel : BaseChamberlainModel
    {
        public DeviceModel()
        {
            thing_id = 0;
            thing_name = string.Empty;
            icon = string.Empty;
            items = new List<NewItemModel>();
            status = DeviceStatus.OK.ToString("G");
        }

        public enum DeviceStatus
        {
            OK,
            Warning,
            Dead
        }

        public string device_db_type { get; set; }

        public byte? device_type { get; set; }

        public long? home_id { get; set; }

        public List<SceneModel> scenes { get; set; }

        public string icon { get; set; }

        public bool is_controller { get; set; }

        public List<NewItemModel> items { get; set; }

        public string native_name { get; set; }

        public int? node_id { get; set; }

        public string status { get; set; }

        public long thing_id { get; set; }

        public string thing_name { get; set; }
    }
}