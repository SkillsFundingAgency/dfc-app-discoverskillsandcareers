using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Services.SessionIdToCodeConverters
{
    public class DefaultSessionIdToCodeConverter : ISessionIdToCodeConverter
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
            var result = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(code))
            {
                foreach (var c in code)
                {
                    if (c != ' ')
                    {
                        result.Append(c.ToString().ToLower());
                    }
                }
            }

            return result.ToString();
        }
    }
}
