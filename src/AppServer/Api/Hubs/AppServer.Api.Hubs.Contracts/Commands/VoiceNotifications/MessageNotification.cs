namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications
{
    public class MessageNotification : VoiceNotification
    {
        public string Subject {get; set;}
        public string Text {get; set;}
        public string From {get; set;}
        public string To {get; set;}

        public MessageNotification(string userName, string from, string to, string subject, string text)
        {
            UserName = userName;
            From = from;
            To = to;
            Subject = subject;
            Text = text;
        }

        public override string GetInformationText()
        {
            return $"I have a message for {To} from {From} titled: {Subject}. It says: {Text}. {To}, do you copy?";
        }

        public override string GetVoipInformationText()
        {
            return $"Hello, it's Chamberlain. I have a message for you from {From}. It's titled: {Subject} and it says: {Text}. {To}. Let me repeat. I have a message for you from {From}. It's titled: {Subject} and it says: {Text}. {To}.";
        }
    }
}
