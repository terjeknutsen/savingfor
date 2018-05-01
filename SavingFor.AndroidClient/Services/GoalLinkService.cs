using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using SavingFor.AndroidClient.Broadcasts;
using SavingFor.AndroidClient.Factories;
using SavingFor.AndroidClient.Settings;

namespace SavingFor.AndroidClient.Services
{
    [Service]
    public sealed class GoalLinkService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            var bundle = intent.GetBundleExtra(nameof(Domain.Model.Goal));
            var ids = bundle.GetStringArray(nameof(Domain.Model.Goal.Id));
            var groupName = bundle.GetString(nameof(Domain.Model.Goal.Group));
            var allGroups = bundle.GetStringArray(nameof(Preferences));

            bool found = false;
            foreach(var group in allGroups)
            {
                if (group.ToUpperInvariant().Equals(groupName.ToUpperInvariant()))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var newGroupList = new List<string>(allGroups);
                newGroupList.Add(groupName);
                Preferences.UsedGroups = newGroupList;
            }

            var repository = RepositoryFactory.GetSingleton().GetRepository();

            foreach(var id in ids)
            {
                var goal = repository.Get(new Guid(id));
                goal.Group = groupName;
                repository.Update(goal);
            }
            Preferences.CurrentGroup = groupName;
            var receiverIntent = new Intent(nameof(GroupCreatedReceiver));
            var receiverBundle = new Bundle();
            receiverBundle.PutString(nameof(Domain.Model.Goal.Group), groupName);
            receiverIntent.PutExtra(nameof(Domain.Model.Goal), bundle);
            LocalBroadcastManager.GetInstance(this).SendBroadcast(receiverIntent);
        }
    }
}