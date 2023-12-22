namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications
{
    public class DoorbellNotification : VoiceNotification
    {
        public override string GetInformationText()
        {
            return $"Somebody rang the doorbell, do you copy?";
        }

        public override string GetVoipInformationText()
        {
            return GetInformationText();
        }
    }
}
