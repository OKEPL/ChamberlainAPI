namespace Chamberlain.AppServer.Api.Contracts.DataTransfer
{
    using Chamberlain.AppServer.Api.Contracts.Models;

    public class CheckResultModel : BaseChamberlainModel
    {
        public bool Exists { get; set; }
    }
}