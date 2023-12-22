namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class ModePostModel : BaseChamberlainModel
    {
        [Required]
        [RegularExpression(StaticExpressions.NoGaps)]
        public string Color { get; set; }

        [Required]
        [RegularExpression(StaticExpressions.StringWithNumbersAndGaps)]
        public string Name { get; set; }
    }
}