using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidClient.Broadcasts;
using SavingFor.AndroidClient.Factories;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Services
{
    [Service]
    public sealed class GoalUnlinkService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            var bundle = intent.GetBundleExtra(nameof(Goal));
            var ids = bundle.GetStringArray(nameof(Goal.Id));

            var repository = RepositoryFactory.GetSingleton().GetRepository();

            foreach(var id in ids)
            {
                var goal = repository.Get(new Guid(id));
                goal.Group = string.Empty;
                repository.Update(goal);
            }

            var receiverIntent = new Intent(nameof(GroupLinkRemovedReceiver));
            LocalBroadcastManager.GetInstance(this).SendBroadcast(receiverIntent);


        }
    }
}