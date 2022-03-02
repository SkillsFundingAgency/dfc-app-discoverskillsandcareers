namespace DFC.App.DiscoverSkillsCareers.Core.Helpers
{
    public static class GeneralHelper
    {
        public static string? GetGenericSkillName(string? socSkillsMatrixName)
        {
            if (socSkillsMatrixName?.IndexOf("-") != 5)
            {
                return socSkillsMatrixName;
            }

            return socSkillsMatrixName?[6..];
        }
    }
}