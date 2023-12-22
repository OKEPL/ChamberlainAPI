namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using Chamberlain.AppServer.Api.Contracts.DataTransfer;

    #endregion

    public interface IRtspPortService
    {
        CheckResultModel CheckHostPort(string ip, int port);

        CheckResultModel CheckRtspPort(string ip, int port);
    }
}