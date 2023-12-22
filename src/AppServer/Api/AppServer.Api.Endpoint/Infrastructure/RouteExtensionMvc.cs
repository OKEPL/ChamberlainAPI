namespace AppServer.Api.Endpoint.Infrastructure
{
    #region

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;

    #endregion

    /// <summary>
    /// The route extension mvc.
    /// </summary>
    public static class RouteExtensionMvc
    {
        /// <summary>
        /// The use central route prefix.
        /// </summary>
        /// <param name="opts">
        /// The opts.
        /// </param>
        /// <param name="routeAttribute">
        /// The route attribute.
        /// </param>
        public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Insert(0, new RouteConvention(routeAttribute));
        }
    }
}