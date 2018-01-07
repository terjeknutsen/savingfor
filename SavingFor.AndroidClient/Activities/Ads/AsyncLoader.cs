using System.Threading;
using System.Threading.Tasks;
using Android.Gms.Ads;
using SavingFor.LPI;

namespace SavingFor.AndroidClient.Activities.Ads
{
    class AsyncLoader
    {
        private readonly AdView view;
        private readonly TaskScheduler taskScheduler;

        public AsyncLoader(AdView view,TaskScheduler taskScheduler)
        {
            this.view = view;
            this.taskScheduler = taskScheduler;
        }

        public Task LoadAdViewAsync(CancellationToken token)
        {
            var adRequest = new AdRequest.Builder().Build();
            return Task.Factory.StartNew(() => view.LoadAd(adRequest), token, TaskCreationOptions.LongRunning, taskScheduler);
        }
    }
}