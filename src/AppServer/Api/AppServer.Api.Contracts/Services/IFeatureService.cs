namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using System.Collections.Generic;

    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Feature;

    #endregion

    public interface IFeatureService
    {
        List<FeatureModel> GetAll(string userName);

        FeatureModel GetFeatureById(long featureId);
    }
}