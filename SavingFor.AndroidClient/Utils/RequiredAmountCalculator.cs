using System.Collections.Generic;
using System.Linq;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Utils
{
    public sealed class RequiredAmountCalculator
    {
        public RequiredAmountCalculator(IEnumerable<Goal> goals)
        {
            RequiredAmounts = goals.Select(g => new GoalAmountRequired(g.ProgressPerMinute, g.Start));
        }

        public IEnumerable<GoalAmountRequired> RequiredAmounts { get; }

        public decimal Calculate()
        {
            return RequiredAmounts.Sum(r => r.RequiredAmount());
        }
    }
}