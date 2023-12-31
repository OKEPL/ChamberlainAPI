﻿namespace AppServer.Api.Endpoint.Infrastructure
{
    #region

    using System.Linq;

    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Routing;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// The route convention.
    /// </summary>
    public class RouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// The _central prefix.
        /// </summary>
        private readonly AttributeRouteModel centralPrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteConvention"/> class.
        /// </summary>
        /// <param name="routeTemplateProvider">
        /// The route template provider.
        /// </param>
        public RouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            this.centralPrefix = new AttributeRouteModel(routeTemplateProvider);
        }

        /// <inheritdoc />
        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                            this.centralPrefix,
                            selectorModel.AttributeRouteModel);
                    }
                }

                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (!unmatchedSelectors.Any())
                {
                    continue;
                }

                foreach (var selectorModel in unmatchedSelectors)
                {
                    selectorModel.AttributeRouteModel = this.centralPrefix;
                }
            }
        }
    }
}