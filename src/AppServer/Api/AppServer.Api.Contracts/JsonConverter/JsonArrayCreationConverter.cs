namespace Chamberlain.AppServer.Api.Contracts.JsonConverter
{
    #region

    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    #endregion

    public abstract class JsonArrayCreationConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jsonArray = JArray.Load(reader);
            var target = this.Create(objectType, jsonArray);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected abstract T Create(Type objectType, JArray jsonArray);
    }
}