using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Core
{
    public static class ByteExtensions
    {
        public static string AsString(this byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }
    }
}