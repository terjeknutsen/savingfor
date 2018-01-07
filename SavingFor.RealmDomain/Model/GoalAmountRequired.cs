using System;

namespace SavingFor.RealmDomain.Model
{
    public struct GoalAmountRequired
    {
        private readonly DateTimeOffset startDateTime;

        public GoalAmountRequired(double incomePerMinute, DateTimeOffset startDateTime)
        {
            ProgressPerMinute = incomePerMinute;
            this.startDateTime = startDateTime;
        }

        public double ProgressPerMinute { get; }

        public double RequiredAmount()
        {
            var totalMinutesPassed = (DateTime.Now - startDateTime).TotalMinutes;
            return ProgressPerMinute * totalMinutesPassed;
        }

        public double RequiredAmountPerDay(int hours = 24, int minutes = 60)
        {
            return ProgressPerMinute * hours * minutes;
        }
    }
}
