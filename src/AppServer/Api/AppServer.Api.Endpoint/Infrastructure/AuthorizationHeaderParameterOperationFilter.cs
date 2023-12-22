
namespace Chamberlain.AppServer.Api.Endpoint.Infrastructure
{
    #region

    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Authorization;

    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    #endregion

    /// <summary>
    /// The authorization header parameter operation filter
    /// </summary>
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// The apply operation and operation filter
        /// </summary>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is IAllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null) operation.Parameters = new List<IParameter>();

                operation.Parameters.Add(
                    new NonBodyParameter
                        {
                            Name = "Authorization",
                            In = "header",
                            Description = "access token",
                            Required = true,
                            Type = "string"
                        });
            }
        }
    }
}