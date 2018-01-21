using Android.Content;
using SavingFor.AndroidClient.Interfaces;

namespace SavingFor.AndroidClient.Broadcasts
{
    public sealed class GroupLinkRemovedReceiver : BroadcastReceiver
    {
        private readonly IHandleGoal handler;

        public GroupLinkRemovedReceiver(IHandleGoal handler)
        {
            this.handler = handler;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            handler.HandleGroupLinkRemoved();
        }
    }
}