using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SavingFor.Domain.Model;

namespace SavingFor.Services
{
    public sealed class TotalUpdater: IDisposable
    {
        private List<GoalAmountRequired> goalAmountRequireds = new List<GoalAmountRequired>(); 
        private readonly Action<decimal> onUpdate;
        private Timer timer;
        private int? delay;
        private bool disposed;

        public TotalUpdater(Action<decimal> onUpdate)
        {
            this.onUpdate = onUpdate;
        }
        ~TotalUpdater()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if(timer != null)
                    timer.Dispose();
                }
                goalAmountRequireds.Clear();
                disposed = true;
            }
        }

        public void Start()
        {
            if (timer != null)
            {
                timer.Change(new TimeSpan(0), new TimeSpan(0, 0, 0, 0, Delay));
            }
            else
                timer = new Timer(DoUpdate,null,new TimeSpan(0), new TimeSpan(0,0,0,0,Delay));
        }

        private void DoUpdate(object state)
        {
            onUpdate(Sum);
        }

        private decimal Sum
        {
            get
            {
                var sum = goalAmountRequireds.Sum(g => g.RequiredAmount());
                return sum;
            }
        }

        public void Reset()
        {
            goalAmountRequireds.Clear();
        }

        public void Stop()
        {
            timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

      
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Set(IEnumerable<GoalAmountRequired> requiredAmounts)
        {
            delay = null;
            goalAmountRequireds = new List<GoalAmountRequired>(requiredAmounts);; 
        }

    

        public int Delay => (int)(delay ?? (delay = CalculateDelay()));
        public decimal Value => Sum;

        private int CalculateDelay()
        {
            var progressPerSecond = goalAmountRequireds.Sum(g => g.ProgressPerMinute) / 60m;
            if (progressPerSecond == 0) return Timeout.Infinite;
            var calculated = (int)(0.01m / progressPerSecond * 1000);

            return calculated > 60 ? calculated : 60;
        }
    }
}
