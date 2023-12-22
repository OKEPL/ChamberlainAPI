using System.Collections.Generic;

namespace Chamberlain.AppServer.Api.Contracts.JsonConverter
{
    using System;
    using System.Linq;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;
    using Chamberlain.AppServer.Api.Contracts.ValidationAttribute;
    using Newtonsoft.Json.Linq;

    public class JsonNotificationConverter : JsonArrayCreationConverter<List<BaseNotificationModel>>
    {
        protected override List<BaseNotificationModel> Create(Type objectType, JArray jsonArray)
        {
            var result = new List<BaseNotificationModel>();
            foreach (var jsonObject in jsonArray.Children<JObject>())
            {
                foreach (var type in NotificationTypes.Types)
                {
                    if (!(type.GetCustomAttributes(typeof(KeyWordAttribute), true).FirstOrDefault() is KeyWordAttribute
                        keyWordAttribute))
                    {
                        throw new ArgumentNullException(
                            $"Class {type} doesn't have KeyWord attribute which is essential for ModelBinding");
                    }

                    foreach (var jsonProperty in jsonObject.Properties())
                    {
                        if (string.Equals(jsonProperty.Name, keyWordAttribute.KeyWord, StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.Add(jsonObject.ToObject(type) as BaseNotificationModel);
                        }
                    }
                }
            }

            return result;
        }
    }
}