using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;

namespace DFC.App.DiscoverSkillsCareers.Services.DataProcessors
{
    public class GetQuestionResponseDataProcessor : BaseDataProcessor, IDataProcessor<GetQuestionResponse>
    {
        public void Processor(GetQuestionResponse value)
        {
            if (value == null)
            {
                return;
            }

            Process(value);
        }
    }
}
