using System;
using SavingFor.Domain.Model;

namespace SavingFor.Domain.Service
{
    public class GoalProgressService
    {
        public decimal ProgressPerminute(Period period, decimal amount)
        {

            return Math.Round(IncomePerMinute(period, amount), 10);
        }

        public decimal IncomePerMinute(Period period, decimal totalIncome)
        {
            if (IsSpecialCase(period, 0, ref totalIncome))
            {
                return totalIncome;
            }
            return totalIncome / (decimal)period.TotalMinutes;
        }

        private static bool IsSpecialCase(Period period, int checkValue, ref decimal amount)
        {
            if (amount > 0m && period.IsActive)
            {
                return (period.MinutesLeft - checkValue) < 0.1;
            }
            amount = 0m;
            return true;
        }
    }
}
