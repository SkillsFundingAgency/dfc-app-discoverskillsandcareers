﻿using System;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class ShortQuestion
    {
        public Guid? Id { get; set; }

        public string? QuestionText { get; set; }

        public bool IsNegative { get; set; }

        public int? Ordinal { get; set; }

        public QuestionAnswer? Answer { get; set; }

        public string? Trait { get; set; }
    }
}