using Chamberlain.AppServer.Api.Contracts;
using Newtonsoft.Json;

namespace Chamberlain.AppServer.Api.Endpoint.Infrastructure.Validation
#pragma warning disable 1591
{
    public class ValidationError
    {
        public ValidationError(string name, string errorMessage)
        {
            Name = name != string.Empty ? name : null;
            Description = ErrorMessageDescription(errorMessage);
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; }

        public string Description { get; }

        private static string ErrorMessageDescription(string errorMessage)
        {
            return errorMessage.Contains(StaticExpressions.UserNameConvention) 
                ? "Invalid username or password" : errorMessage;
        }
    }
}