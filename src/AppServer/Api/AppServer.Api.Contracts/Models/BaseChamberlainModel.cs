namespace Chamberlain.AppServer.Api.Contracts.Models
{
    #region

    using Chamberlain.AppServer.Api.Contracts.JsonConverter;

    using Newtonsoft.Json;

    #endregion

    [JsonConverter(typeof(JsonModelConverter))]
    public abstract class BaseChamberlainModel
    {
    }
}