using System;
using Android.App;
using Android.OS;
using Android.Widget;
using SavingFor.AndroidClient.Interfaces;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Dialogs
{
    public sealed class LinkDialog : Android.Support.V4.App.DialogFragment
    {
        private AutoCompleteTextView editText;
        private ILinkGoalGroup groupLinker;

        public static LinkDialog NewInstance(Bundle bundle)
        {
            return new LinkDialog()
            {
                Arguments = bundle
            };
        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var layoutInflater = Activity.LayoutInflater;
            var view = layoutInflater.Inflate(Resource.Layout.dialog_select_link, null);
            editText = view.FindViewById<AutoCompleteTextView>(Resource.Id.dialog_select_link_edit);
            groupLinker = Activity as ILinkGoalGroup;
            if (groupLinker == null)
                throw new NullReferenceException($"{Activity.Class.SimpleName} does not implement {nameof(ILinkGoalGroup)}");
            var groupNames = Arguments.GetStringArray(nameof(Goal.Group));
            var adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, groupNames);
            editText.Adapter = adapter;
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle(Resource.String.dialog_title_link);
            builder.SetView(view);
            builder.SetPositiveButton(Resource.String.dialog_ok_button, (o, e) => OnOkClick(o, e));
            return builder.Create();
        }

        private void OnOkClick(object sender, EventArgs e)
        {
            if (editText.Text != null)
                groupLinker.LinkGroup(editText.Text);
            Dismiss();
        }

    }
}