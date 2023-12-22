using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device
{
    public class ForceDeviceUpdateModel
    {
        [Required]
        public long ThingId { get; set; }
    }
}
