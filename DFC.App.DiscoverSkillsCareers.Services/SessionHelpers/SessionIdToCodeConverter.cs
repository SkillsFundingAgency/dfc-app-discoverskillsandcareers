using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Services.SessionHelpers
{
    public class SessionIdToCodeConverter : ISessionIdToCodeConverter
    {
        public string GetCode(string value)
        {
            var result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Trim().ToUpper();
                for (var i = 0; i < value.Length; i++)
                {
                    if (i % 4 == 0 && i > 1)
                    {
                        result.Append(" ");
                    }

                    result.Append(value[i]);
                }
            }

            return result.ToString();
        }

        public string GetSessionId(string code)
        {
            var result = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(code))
            {
                code = code.ToLower();
                foreach (var c in code)
                {
                    if (c != ' ')
                    {
                        result.Append(c.ToString());
                    }
                }
            }

            return result.ToString();
        }
    }
}
