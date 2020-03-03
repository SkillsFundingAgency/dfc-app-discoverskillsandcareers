using System;
using System.Runtime.Serialization;

namespace DFC.App.DiscoverSkillsCareers.Core.Exceptions
{
    [Serializable]
    public class SessionNotFoundException : Exception
    {
        public SessionNotFoundException()
        {
        }

        public SessionNotFoundException(string message)
            : base(message)
        {
        }

        public SessionNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SessionNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}