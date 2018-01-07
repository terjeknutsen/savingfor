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

        public ActionBarCallback(Activity activity, Action onDelete, Action onCancel, Action onShowMonthlyPlan)
        {
            this.activity = activity;
            this.onDelete = onDelete;
            this.onCancel = onCancel;
            this.onShowMonthlyPlan = onShowMonthlyPlan;
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