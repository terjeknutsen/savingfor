using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SavingFor.LPI;
using SavingFor.RealmDomain.Model;

namespace SavingFor.RealmDomain.Service
{
    public sealed class PlanFactory
    {
        private readonly ILogger debugLogger;
        private readonly Dictionary<Goal, GoalAmountRequired> goalsAndProgress;

        public PlanFactory(IEnumerable<Goal> goals, ILogger debugLogger)
        {
            this.debugLogger = debugLogger;
            goalsAndProgress = goals.ToDictionary(g => g, goal => new GoalAmountRequired(goal.ProgressPerMinute, goal.Start));
        }

        public IEnumerable<MonthlyPlan> CreatePlan()
        {
            var sums = new Dictionary<DateTime, double>();
            int maxMonthCount;
            maxMonthCount = 12;
            for (var i = 0; i < maxMonthCount; i++)
            {
                var month = i;

                var goals = goalsAndProgress.Keys.Where(g => (g.End.Year >= DateTime.Now.Year && g.End.Month >= DateTime.Now.AddMonths(month).Month) || g.End.Year > DateTime.Now.Year);

                foreach (var goal in goals)
                {
                    if (goal.End.Year < DateTime.Now.AddMonths(month).Year) continue;
                    if (goal.End.Year <= DateTime.Now.Year && goal.End.Month < DateTime.Now.AddMonths(month).Month) continue;
                    var period = new Period(goal.Start, goal.End);
                    var totalMinutes = period.TotalMinutesWithinMonth(DateTime.Now.AddMonths(month));
                    AddSum(sums, DateTime.Now.AddMonths(month).Date, totalMinutes, goal);
                    LogGoal(goal, sums, totalMinutes);
                }
            }

            return sums.Keys.Select(key => new MonthlyPlan
            {
                Month = key.ToString("MMMM"),
                Year = key.Year,
                Amount = sums[key]
            });
        }
        [Conditional("DEBUG")]
        private void LogGoal(Goal goal, Dictionary<DateTime, double> sums, double totalMinutes)
        {
            var logMessageBuilder = new StringBuilder();
            logMessageBuilder.Append($"Goal: {goal.Name}, period: {goal.Start.ToString("G")} - {goal.End.ToString("G")}");
            logMessageBuilder.Append(Environment.NewLine);
            logMessageBuilder.Append($"Progress : {goal.ProgressPerMinute} - minutes in month: {totalMinutes}");
            logMessageBuilder.Append(Environment.NewLine);
            logMessageBuilder.Append($"Sum: {sums.Last().Value}");
            logMessageBuilder.Append(Environment.NewLine);
            debugLogger.Log(logMessageBuilder.ToString());
        }

        private void AddSum(IDictionary<DateTime, double> sums, DateTime date, double totalMinutes, Goal goal)
        {
            if (!sums.ContainsKey(date))
            {
                sums.Add(date, 0d);
            }

            sums[date] += totalMinutes * goalsAndProgress[goal].ProgressPerMinute;
        }
    }
}
