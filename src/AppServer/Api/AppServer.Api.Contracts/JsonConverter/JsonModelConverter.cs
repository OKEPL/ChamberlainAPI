namespace Chamberlain.AppServer.Api.Contracts.JsonConverter
{
    #region

    using System;

    using Newtonsoft.Json;

    #endregion

    public class JsonModelConverter : JsonConverter
    {
        private const string CutOffLower = "model";

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            writer.WriteStartObject();

            foreach (var property in value.GetType().GetProperties())
            {
                string propertyName;
                if (property.Name.ToLower().Contains(CutOffLower))
                {
                    var index = property.Name.ToLower().IndexOf(CutOffLower, StringComparison.Ordinal);

                    // write property name wihout CutOff phrase
                   propertyName = property.Name.Remove(index, CutOffLower.Length);
                }
                else
                {
                    propertyName = property.Name;
                }

                writer.WritePropertyName(ConvertStringToLower(propertyName));
                var propertyValue = property.GetValue(value);

                serializer.Serialize(writer, propertyValue);
            }

            writer.WriteEndObject();
        }

        private static string ConvertStringToLower(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}