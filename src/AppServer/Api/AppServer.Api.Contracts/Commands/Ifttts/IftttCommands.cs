namespace Chamberlain.AppServer.Api.Contracts.Commands.Ifttts
{
    public class TriggerAction
    {
        public TriggerAction(string userLogin, long actionId)
        {
            this.UserLogin = userLogin;
            this.ActionId = actionId;
        }

        public long ActionId { get; }

        public string UserLogin { get; }
    }
}