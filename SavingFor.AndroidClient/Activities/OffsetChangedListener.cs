using System;
using Android.Support.Design.Widget;

namespace SavingFor.AndroidClient.Activities
{
    public class OffsetChangedListener : Java.Lang.Object, AppBarLayout.IOnOffsetChangedListener
    {
        private readonly Action<int> onCollapsed;
        private readonly Action<int> onExpanded;
        private bool disposed= false;

        public OffsetChangedListener(Action<int> onCollapsed, Action<int> onExpanded)
        {
            this.onCollapsed = onCollapsed;
            this.onExpanded = onExpanded;
        }

        public void OnOffsetChanged(AppBarLayout layout, int verticalOffset)
        {
            if (verticalOffset > -100)
                onExpanded(verticalOffset);
            else
            {
                onCollapsed(verticalOffset);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}