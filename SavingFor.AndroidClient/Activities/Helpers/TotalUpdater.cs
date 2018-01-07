using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Widget;
using SavingFor.Domain.Model;
using Timer = System.Timers.Timer;

namespace SavingFor.AndroidClient.Activities.Helpers
{
    internal sealed class TotalUpdater
    {
        private readonly IEnumerable<GoalAmountRequired> progresses;

        public TotalUpdater(IEnumerable<Goal> items)
        {
            progresses = items.Select(g => new GoalAmountRequired(g.ProgressPerMinute, g.Start));
        }

        private Timer timer = new Timer();

        public void Update(Action<decimal> onUpdate)
        {
             
        }

        public void StartUpdating(TextView view, CancellationToken token, Action onUpdate)
        {
            Task.Factory.StartNew(async() =>
            {
                while (updating)
                {
                    var amount = progresses.Sum(r => r.RequiredAmount());
                    view.Text = $"{amount.ToString("C")}";
                    await Task.Delay(Delay, token);
                    token.ThrowIfCancellationRequested();
                    onUpdate();
                }
            },token);
            
        }

        public void Clear()
        {
            updating = false;
        }

        private int? delay;
        private bool updating = true;
        public int Delay => (int)(delay ?? (delay = CalculateDelay()));

        private int CalculateDelay()
        {
            var progressPerSecond = progresses.Sum(g => g.ProgressPerMinute) / 60m;
            var calculated = (int)(0.01m / progressPerSecond * 1000);

            return calculated > 60 ? calculated : 60;
        }
    }
}