namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Features;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Feature;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Features controller class.
    /// </summary>
    [Route("feature")]
    public class FeaturesController : ChamberlainBaseController
    {
        /// <summary>
        /// Gets feature using given Id.
        /// </summary>
        /// <param name="id">
        /// Id of feature we ask for.
        /// </param>
        /// <returns>
        /// Feature model.
        /// </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FeatureModel), 200)]
        public async Task<FeatureModel> GetFeatureById(long id)
        {
            return await SystemActors.FeatureActor.Execute<GetFeatureById, FeatureModel>(new GetFeatureById(id));
        }

        /// <summary>
        /// Get list of all features for current user.
        /// </summary>
        /// <returns>
        /// List of feature models.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<FeatureModel>), 200)]
        public async Task<List<FeatureModel>> GetFeatures()
        {
            return await SystemActors.FeatureActor.Execute<GetAll, List<FeatureModel>>(new GetAll(this.User.Identity.Name));
        }
    }
}