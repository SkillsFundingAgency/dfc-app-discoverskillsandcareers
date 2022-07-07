using HashidsNet;
using System;
using System.IO;

namespace DFC.App.DiscoverSkillsCareers.Core.Helpers
{
    public static class SessionIdHelper
    {
        private const string UsableChars = "acefghjkmnrstwxyz23456789";
        private static readonly object SyncLock = new object();

        // ReSharper disable once InconsistentNaming
        private static int lockedCounter = 10;

        public static string GenerateSessionId(string salt) => GenerateSessionId(salt, DateTime.UtcNow);

        public static string GenerateSessionId(string salt, DateTime date)
        {
            var hashids = new Hashids(salt, 4, UsableChars);
            var rand = Counter();
            var year = (date.Year - 2018).ToString();
            var digits = Convert.ToInt64($"{year}{date:MMddHHmmssfff}{rand}");
            var code = hashids.EncodeLong(digits);
            var decode = Decode(salt, code);

            if (digits.ToString() != decode)
            {
                throw new InvalidDataException("Invalid decode");
            }

            return code;
        }

        public static string Decode(string salt, string code)
        {
            var hashids = new Hashids(salt, 4, UsableChars);
            var decode = hashids.DecodeLong(code);

            return decode.Length > 0 ? decode[0].ToString() : null;
        }

        private static int Counter()
        {
            lock (SyncLock)
            {
                if (lockedCounter >= 99)
                {
                    lockedCounter = 0;
                }

                lockedCounter++;
                return lockedCounter;
            }
        }
    }
}
