using Android.Content;
using SavingFor.AndroidClient.Interfaces;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Broadcasts
{
    public sealed class GroupCreatedReceiver : BroadcastReceiver
    {
        private readonly IHandleGoal handler;

        public GroupCreatedReceiver(IHandleGoal handler)
        {
            this.handler = handler;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            var bundle = intent.GetBundleExtra(nameof(Goal));
            handler.HandleGoalGroupCreated(bundle.GetString(nameof(Goal.Group)));
        }
    }
}