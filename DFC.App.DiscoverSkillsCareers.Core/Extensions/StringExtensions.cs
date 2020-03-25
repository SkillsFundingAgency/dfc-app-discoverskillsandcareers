using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Core.Extensions
{
    public static class StringExtensions
    {
        public static byte[] AsByteArray(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
