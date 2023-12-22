using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device
{
    public class UpdateDeviceModel : BaseChamberlainModel
    {
        [Required]
        public long ThingId { get; set; }
        
        [RegularExpression(StaticExpressions.StringWithNumbersAndGaps)]
        public string DeviceName { get; set; }
    }
}