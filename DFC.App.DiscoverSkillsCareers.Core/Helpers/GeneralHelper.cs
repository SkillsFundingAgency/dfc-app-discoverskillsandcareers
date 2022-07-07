namespace DFC.App.DiscoverSkillsCareers.Core.Helpers
{
    public static class GeneralHelper
    {
        public static string GetGenericSkillName(string socSkillsMatrixName)
        {
            return socSkillsMatrixName?.IndexOf("-") != 5 ? socSkillsMatrixName : socSkillsMatrixName?[6..];
        }
    }
}