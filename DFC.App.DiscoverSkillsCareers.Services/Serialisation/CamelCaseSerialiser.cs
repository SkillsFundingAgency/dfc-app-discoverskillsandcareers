using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFC.App.DiscoverSkillsCareers.Services
{
    public class CamelCaseSerialiser : ISerialiser
    {
        public T Deserialise<T>(string value)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return JsonConvert.DeserializeObject<T>(value, serializerSettings);
        }

        public string Serialise(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
