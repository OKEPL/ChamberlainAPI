namespace Chamberlain.AppServer.Api.Contracts.Services
{
    public interface IIftttService
    {
        void TriggerAction(string userLogin, long actionId);
    }
}