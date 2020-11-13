using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class CustomLinkDetailConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ILinkDetails);
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            JsonSerializer se = new JsonSerializer();

            var result = se.Deserialize<CustomLinkDetails>(reader);

            if (result == null)
            {
                throw new InvalidOperationException(nameof(result));
            }

            return result;
        }
    }
}