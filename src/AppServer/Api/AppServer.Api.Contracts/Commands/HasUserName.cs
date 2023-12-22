namespace Chamberlain.AppServer.Api.Contracts.Commands
{
    public abstract class HasUserName
    {
        protected HasUserName(string userName)
        {
            this.UserName = userName;
        }

        public string UserName { get; }
    }
}