namespace AppServer.Api.Endpoint.Controllers
{
    #region

    using Chamberlain.AppServer.Api.Endpoint.Infrastructure.Validation;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// The chamberlain base controller.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ValidateModel]
    public class ChamberlainBaseController : Controller
    {
        /// <summary>
        /// Gets the base routing.
        /// </summary>
        public static string BaseRouting { get; } = "api/v2";
    }
}