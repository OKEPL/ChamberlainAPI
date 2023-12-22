using Chamberlain.AppServer.Api.Endpoint.Helpers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chamberlain.AppServer.Api.Endpoint.Infrastructure.Validation
#pragma warning disable 1591
{
    public class ValidationResultModel
    {    
        public ValidationResultModel(ModelStateDictionary modelState)
        {
            Errors = modelState.Keys.SelectMany(
                    key => modelState[key].Errors
                        .Select(x => new ValidationError(key.FirstCharToLower(), x.ErrorMessage)))
                .ToList();
        }

        public List<ValidationError> Errors { get; }
    }
}