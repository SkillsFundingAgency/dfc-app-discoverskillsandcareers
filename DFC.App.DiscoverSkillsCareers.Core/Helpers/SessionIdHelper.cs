using HashidsNet;
using System;
using System.IO;

namespace DFC.App.DiscoverSkillsCareers.Core.Helpers
{
    public static class SessionIdHelper
    {
        private const string Alphabet = "acefghjkmnrstwxyz23456789";
        private static readonly object SyncLock = new object();
        private static int counter = 10;

        public static string GenerateSessionId(string salt, DateTime date)
        {
            var hashids = new Hashids(salt, 4, Alphabet);
            int rand = Counter();
            string year = (date.Year - 2018).ToString();
            long digits = Convert.ToInt64($"{year}{date.ToString("MMddHHmmssfff")}{rand}");
            var code = hashids.EncodeLong(digits);
            var decode = Decode(salt, code);
            if (digits.ToString() != decode)
            {
                throw new InvalidDataException("Invalid decode");
            }

            return code;
        }

        public static string GenerateSessionId(string salt) => GenerateSessionId(salt, DateTime.UtcNow);

        public static string Decode(string salt, string code)
        {
            var hashids = new Hashids(salt, 4, Alphabet);
            var decode = hashids.DecodeLong(code);
            if (decode.Length > 0)
            {
                return decode[0].ToString();
            }

            return null;
        }

        public static int Counter()
        {
            lock (SyncLock)
            {
                if (counter >= 99)
                {
                    counter = 0;
                }

                counter++;
                return counter;
            }
        }
    }
}
