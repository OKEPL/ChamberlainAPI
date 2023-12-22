namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications
{
    public class SlipOverNotification : VoiceNotification
    {
        public string PersonName { get; set; }

        public SlipOverNotification(string personName)
        {
            PersonName = personName;
        }

        public override string GetInformationText()
        {
            return $"I saw you have slipped over. {PersonName}, is everything okay?";
        }

        public override string GetVoipInformationText()
        {
            return GetInformationText();
        }
    }
}
