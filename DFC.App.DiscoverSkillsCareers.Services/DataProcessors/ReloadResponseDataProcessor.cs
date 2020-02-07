using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Services.DataProcessors
{
    public class ReloadResponseDataProcessor : IDataProcessor<ReloadResponse>
    {
        public void Processor(ReloadResponse value)
        {
            if (value == null)
            {
                return;
            }

            value.ReferenceCode = GetReferenceCode(value.ReloadCode);
        }

        private string GetReferenceCode(string reloadCode)
        {
            var result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(reloadCode))
            {
                reloadCode = reloadCode.Trim().ToUpper();
                int i = 0;
                foreach (var c in reloadCode.ToCharArray())
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
    }
}
