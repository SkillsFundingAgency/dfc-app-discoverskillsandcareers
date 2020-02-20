using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;

namespace DFC.App.DiscoverSkillsCareers.Services.DataProcessors
{
    public class GetAssessmentResponseDataProcessor : BaseDataProcessor, IDataProcessor<GetAssessmentResponse>
    {
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;

        public GetAssessmentResponseDataProcessor(ISessionIdToCodeConverter sessionIdToCodeConverter)
        {
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
        }

        public void Processor(GetAssessmentResponse value)
        {
            if (value == null)
            {
                return;
            }

            value.ReferenceCode = sessionIdToCodeConverter.GetCode(value.ReloadCode);

            Process(value);
        }
    }
}
