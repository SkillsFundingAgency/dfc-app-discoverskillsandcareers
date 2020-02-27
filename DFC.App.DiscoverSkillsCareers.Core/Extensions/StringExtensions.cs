using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Core
{
    public static class StringExtensions
    {
        public static byte[] AsByteArray(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}