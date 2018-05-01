using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidClient.Interfaces;
using SavingFor.AndroidClient.Settings;

namespace SavingFor.AndroidClient.Activities
{
    class DefaultActionBarCallback : Java.Lang.Object, ActionMode.ICallback, AdapterView.IOnItemSelectedListener
    {
        private readonly Activity context;
        private readonly ViewGroup viewGroup;
        private readonly IAddGroup groupAdder;
        private readonly IDeleteGoal goalDeleter;
        private readonly ILinkGoalGroup groupLinker;
        private List<string> groups;

        public DefaultActionBarCallback(Activity context,ViewGroup viewGroup,IAddGroup groupAdder,IDeleteGoal goalDeleter,ILinkGoalGroup groupLinker)
        {
            this.context = context;
            this.viewGroup = viewGroup;
            this.groupAdder = groupAdder;
            this.goalDeleter = goalDeleter;
            this.groupLinker = groupLinker;
        }
        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            if(item.ItemId == Resource.Id.item_add_group)
            {
                groupAdder.AddGroup();
            }
            if(item.ItemId == Resource.Id.item_delete)
            {
                goalDeleter.DeleteGoal();
            }
            return false;
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            context.MenuInflater.Inflate(Resource.Menu.cab_default_menu, menu);
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.spinner_group, viewGroup, false);
            var spinner = view.FindViewById<Spinner>(Resource.Id.group_spinner);

            groups = new List<string> { context.GetString(Resource.String.none_selected) };
            groups.AddRange(Preferences.UsedGroups);
            var adapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleListItem1, groups);
            
            spinner.Adapter = adapter;
            mode.CustomView = view;
            
            spinner.OnItemSelectedListener = this;
            return true;
        }


        bool isInSetup = true;
        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            if (isInSetup)
            {
                isInSetup = !isInSetup;
                return;
            }
            var item = groups[position];
            groupLinker.LinkGroup(item);
        }

        public void OnNothingSelected(AdapterView parent)
        {
            
        }
    }
}