using System.ComponentModel.DataAnnotations;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Voice
{
    public class VoiceInterpretationRequestModel : BaseChamberlainModel
    {
        [Required]
        public string Text { get; set; }
    }
}
