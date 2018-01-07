using System;
using SavingFor.RealmDomain.Model;

namespace SavingFor.RealmDomain.Service
{
    public class GoalProgressService
    {
        public double ProgressPerminute(Period period, double amount)
        {

            return Math.Round(IncomePerMinute(period, amount), 10);
        }

        public double IncomePerMinute(Period period, double totalIncome)
        {
            if (IsSpecialCase(period, 0, ref totalIncome))
            {
                return totalIncome;
            }
            return totalIncome / period.TotalMinutes;
        }

        private static bool IsSpecialCase(Period period, int checkValue, ref double amount)
        {
            if (amount > 0d && period.IsActive)
            {
                return (period.MinutesLeft - checkValue) < 0.1;
            }
            amount = 0d;
            return true;
        }
    }
}
