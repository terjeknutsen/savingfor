using System;
using System.Security;

namespace SavingFor.Domain.Model
{
    
    public struct GoalAmountRequired
    {
        private readonly DateTime startDateTime;

        public GoalAmountRequired(decimal incomePerMinute, DateTime startDateTime)
        {
            ProgressPerMinute = incomePerMinute;
            this.startDateTime = startDateTime;
        }

        public decimal ProgressPerMinute { get; }

        public decimal RequiredAmount()
        {
            var totalMinutesPassed = (DateTime.Now - startDateTime).TotalMinutes;
            return ProgressPerMinute * (decimal)totalMinutesPassed;
        }

        public decimal RequiredAmountPerDay(int hours = 24, int minutes = 60)
        {
            return ProgressPerMinute * hours * minutes;
        }
    }
}
