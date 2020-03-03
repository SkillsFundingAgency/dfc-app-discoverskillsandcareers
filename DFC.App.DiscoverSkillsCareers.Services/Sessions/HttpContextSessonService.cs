using DFC.App.DiscoverSkillsCareers.Core.Extensions;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Http;

namespace DFC.App.DiscoverSkillsCareers.Services.Sessions
{
    public class HttpContextSessonService : ISessionService
    {
        private readonly ISession session;
        private readonly ISerialiser serialiser;

        public HttpContextSessonService(IHttpContextAccessor httpContextAccessor, ISerialiser serialiser)
        {
            this.session = httpContextAccessor?.HttpContext?.Session;
            this.serialiser = serialiser;
        }

        public T GetValue<T>(string key)
        {
            var result = default(T);

            if (session != null && session.IsAvailable)
            {
                byte[] sessionValue = null;
                if (session.TryGetValue(key, out sessionValue) && sessionValue != null)
                {
                    var jsonData = sessionValue.AsString();
                    if (!string.IsNullOrWhiteSpace(jsonData))
                    {
                        result = serialiser.Deserialise<T>(jsonData);
                    }
                }
            }

            return result;
        }

        public void SetValue(string key, object value)
        {
            if (session != null && session.IsAvailable)
            {
                var valueSerialized = serialiser.Serialise(value);
                session.Set(key, valueSerialized.AsByteArray());
            }
        }
    }
}
