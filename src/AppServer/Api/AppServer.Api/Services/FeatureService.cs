using System;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Common.Domotica;
using System.Collections.Generic;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Feature;
using Chamberlain.Database.Persistency.Model;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Services
{
    public class FeatureService : IFeatureService
    {
        public FeatureModel GetFeatureById(long featureId)
        {
            using (var context = new Entities())
            {
                var feature = context.Features
                    .FirstOrDefault(f => f.Id == featureId);

                if (feature == null)
                    throw new ArgumentNullException(nameof(featureId));
                
                var details = XmlSerialization.FromString<FeatureDetails>(feature.FeatureDetails);

                return new FeatureModel
                {
                    Id = feature.Id,
                    Name = feature.Name,
                    FeatureType = feature.Type,
                    PriceEur = feature.Price / 100,
                    PriceCen = feature.Price % 100,
                    Description = feature.Description,
                    DiskSpace = details.DiskSpace,
                    Cameras = details.Cameras,
                    Sms = details.Sms,
                    Voip = details.Voip,
                    Hub = details.Hub
                };
            }
        }

        public List<FeatureModel> GetAll(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                var features = context.Features.ToList();

                var current = customer.CustomerFeatureBindings
                        .FirstOrDefault(f => !f.EndDate.HasValue || f.EndDate.Value > DateTime.UtcNow);
                    
                var result = new List<FeatureModel>();
                foreach (var feature in features)
                {
                    var details = XmlSerialization.FromString<FeatureDetails>(feature.FeatureDetails);

                    var featureModel = new FeatureModel
                    {
                        Id = feature.Id,
                        Name = feature.Name,
                        FeatureType = feature.Type,
                        PriceEur = feature.Price / 100,
                        PriceCen = feature.Price % 100,
                        Description = feature.Description,
                        DiskSpace = details.DiskSpace,
                        Cameras = details.Cameras,
                        Sms = details.Sms,
                        Voip = details.Voip,
                        Current = feature.Id == current?.FeatureId,
                        Hub = details.Hub
                    };

                    result.Add(featureModel);
                }

                return result;
            }
        }
    }
}
