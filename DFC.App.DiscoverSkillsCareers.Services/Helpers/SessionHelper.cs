namespace DFC.App.DiscoverSkillsCareers.Services.Helpers
{
    public static class SessionHelper
    {
        public static string FormatSessionId(string referenceCode)
        {
            if (string.IsNullOrEmpty(referenceCode) || referenceCode.Length < 14)
            {
                return referenceCode;
            }

            var referenceCodeUpper = referenceCode.ToUpper();

            return
                referenceCodeUpper.Substring(0, 4) +
                " " +
                referenceCodeUpper.Substring(4, 4) +
                " " +
                referenceCodeUpper.Substring(8, 4) +
                " " +
                referenceCodeUpper.Substring(12, 2);
        }
    }
}