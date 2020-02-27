using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Services
{
    public class SessionIdToCodeConverter : ISessionIdToCodeConverter
    {
        public string GetCode(string value)
        {
            var result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Trim().ToUpper();
                int i = 0;
                foreach (var c in value)
                {
                    i++;
                    if (i % 4 == 1 && i > 1)
                    {
                        result.Append(" ");
                    }

                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public string GetSessionId(string code)
        {
            return code?.Replace(" ", string.Empty).ToLower();
        }
    }
}