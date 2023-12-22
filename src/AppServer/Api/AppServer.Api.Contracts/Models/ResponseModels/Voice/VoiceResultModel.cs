namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Voice
{
    public class VoiceResultModel : BaseChamberlainModel
    {
        public bool DidSucceed { get; set; }
        public string Message { get; set; }

        public static VoiceResultModel Success(string message)
        {
            return new VoiceResultModel {DidSucceed = true, Message = ""};
        }
    }
}
