using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Converters
{
    public class ConcreteTypeConverter<TConcrete> : JsonConverter
        where TConcrete : class, IDysacContentModel
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            var items = serializer.Deserialize<IEnumerable<TConcrete>>(reader!);
            var listToReturn = new List<IDysacContentModel>();
            listToReturn.AddRange(items!);

            return listToReturn;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            serializer.Serialize(writer, value);
        }
    }
}
