namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Device
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class ControllerCommandModel : BaseChamberlainModel
    {
        [RegularExpression(StaticExpressions.StringWithNumbersAndGaps)]
        public string Arg { get; set; }

        [Required]
        public string Command { get; set; }

        [Required]
        public long ThingId { get; set; }
    }
}