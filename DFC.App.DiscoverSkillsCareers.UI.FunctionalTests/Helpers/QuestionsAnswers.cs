using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers
{
    class QuestionsAnswers
    {

        public class QuestionsAndAnswers
        {
            public AnswerData[] Set { get; set; }
        }

        public class AnswerData
        {
            public int Percentprogress { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
        }

    }
}
