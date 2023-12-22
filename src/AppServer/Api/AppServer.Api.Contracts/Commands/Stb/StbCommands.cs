namespace Chamberlain.AppServer.Api.Contracts.Commands.Stb
{
    public class StbLogin : HasUserName
    {
        public string SolocooLogin;

        public StbLogin(string userName, string solocooLogin) : base(userName)
        {
            SolocooLogin = solocooLogin;
        }
    }
}