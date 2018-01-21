using System;
using Android.App;
using Android.Views;

namespace SavingFor.AndroidClient.Activities
{
    class ActionBarCallback : Java.Lang.Object, ActionMode.ICallback
    {
        private readonly Activity activity;
        private readonly Action onDelete;
        private readonly Action onCancel;
        private readonly Action onShowMonthlyPlan;
        private readonly Action onLinkSelected;
        private readonly Action onUnlink;

        public ActionBarCallback(Activity activity, 
            Action onDelete, 
            Action onCancel, 
            Action onShowMonthlyPlan,
            Action onLink,
            Action onUnlink)
        {
            this.activity = activity;
            this.onDelete = onDelete;
            this.onCancel = onCancel;
            this.onShowMonthlyPlan = onShowMonthlyPlan;
            this.onLinkSelected = onLink;
            this.onUnlink = onUnlink;
        }

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            if (item.ItemId == Resource.Id.item_delete)
            {
                onDelete();
            }
            if (item.ItemId == Resource.Id.item_selected_monthly_plan)
            {
                onShowMonthlyPlan();
            }
            if (item.ItemId == Resource.Id.item_link)
            {
                onLinkSelected();
            }
            if (item.ItemId == Resource.Id.item_unlink)
            {
                onUnlink();
            }

            return false;
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            activity.MenuInflater.Inflate(Resource.Menu.cab_menu, menu);
            
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            onCancel();
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            mode.Title = "1";
            return false;
        }


    }
}