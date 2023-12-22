using Chamberlain.Database.Persistency.Model;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands.VoiceNotifications
{
    public class ReminderNotification : VoiceNotification
    {
        public string Subject { get; set; }
        public string Text { get; set; }
        public long Id { get; set; }
        public string ProfileName { get; set; }

        public ReminderNotification(VoiceNote reminder)
        {
            UserName = reminder.FaceProfile.Customer.Username;
            Text = reminder.Text;
            Id = reminder.Id;
            ProfileName = reminder.FaceProfile != null ? reminder.FaceProfile.Name : string.Empty ;
        }

        public override string GetInformationText()
        {
            return $"I have a reminder for {ProfileName}. It says: {Text}. Do you copy?";
        }

        public override string GetVoipInformationText()
        {
            return $"Hello, this is Chamberlain. I have a reminder for you. It says: {Text}. I repeat, the text is {Text}.";
        }
    }
}
