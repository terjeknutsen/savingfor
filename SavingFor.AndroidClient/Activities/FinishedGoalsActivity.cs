using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using SavingFor.AndroidClient.Activities.Ads;
using SavingFor.AndroidClient.Adapters;
using SavingFor.AndroidClient.Factories;
using SavingFor.AndroidClient.Settings;
using SavingFor.AndroidLib;
using SavingFor.AndroidLib.Apis;
using SavingFor.Domain.Model;
using SavingFor.LPI;
using ActionMode = Android.Support.V7.View.ActionMode;
using Android.Support.V4.App;
using Android.Support.V4.Content;

namespace SavingFor.AndroidClient.Activities
{
    [Activity(Label = "FinishedGoalsActivity", Theme = "@style/material_theme_modifiable", ParentActivity = typeof(MainActivity), ScreenOrientation = ScreenOrientation.Portrait)]
    public class FinishedGoalsActivity : AppCompatActivity
    {
        private GoalFinishedAdapter adapter;
        private ActionMode mActionMode;
        private IRepository<Goal> repository;
        private IImageService imageService;
        private RecyclerView recyclerView;
        private Toolbar toolbar;
        private CancellationTokenSource cancellationTokenSource;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            TrySetStatusbarColor();
            repository = RepositoryFactory.GetSingleton().GetRepository();
            imageService = SimpleIoC.GetImageService(this, Resources.DisplayMetrics);
            SetContentView(Resource.Layout.activity_goal_finished_view);
            SetupToolbar();
            SetupRecyclerView();
            await SetItemsAsync();
            await LoadAdsAsync();
        }
        private void TrySetStatusbarColor()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                Window.SetStatusBarColor(Preferences.CurrentColorDark);
        }
        private async Task LoadAdsAsync()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var adView = FindViewById<AdView>(Resource.Id.adView_finished_goals);
            var loader = new AsyncLoader(adView,TaskScheduler.FromCurrentSynchronizationContext());
            await loader.LoadAdViewAsync(cancellationTokenSource.Token);
        }

        private async Task SetItemsAsync()
        {
            try
            {
                var goals = await repository.AllAsync();
               RunOnUiThread(() =>
               {
                   adapter.SetItems(goals.Where(g => g.End < DateTime.Now));
               }); 
            }
            catch (Exception)
            {
                Finish();
            }
            if (adapter.ItemCount > 0)
            {
                FindViewById<CoordinatorLayout>(Resource.Id.coordinatorlayout_finished).Background = null;
            }
            else
            {
                FindViewById<CoordinatorLayout>(Resource.Id.coordinatorlayout_finished)
                  .SetBackgroundResource(Resource.Drawable.background);
            }
        }

        private void SetupToolbar()
        {
             toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_finished);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetTitle(Resource.String.finished);
                var color = new Color(ContextCompat.GetColor(this,Resource.Color.primary)); 
                SupportActionBar.SetBackgroundDrawable(new ColorDrawable(color));
            }
        }

        private void SetupRecyclerView()
        {
            adapter = new GoalFinishedAdapter(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_broken_image_white_48dp), ContentResolver, Resources, imageService);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view_finished);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.SetAdapter(adapter);
            adapter.ItemClick += OnItemClicked;
            adapter.ItemLongClick += OnItemLongClick;
            adapter.EditModeEnded += delegate { OnEditmodeEnded(); };
            adapter.ItemCheckChanged += OnItemCheckChanged;
        }

        private void OnEditmodeEnded()
        {
            mActionMode?.Finish();
        }

        public void OnItemCheckChanged(object sender, int count)
        {
            if (mActionMode == null) return;
            mActionMode.Title = count.ToString("G");
        }

        private void OnItemClicked(object sender, Guid itemId)
        {
            var intent = new Intent(this, typeof(GoalEditActivity));
            intent.PutExtra(GoalEditActivity.GoalId, itemId.ToString());
            StartActivity(intent);
        }

        private void OnItemLongClick(object sender, Guid itemId)
        {
            mActionMode = StartSupportActionMode(new SupportActionBarCallback(this, OnDelete, OnCancel));
        }

        private async void OnCancel()
        {
           await ResetAdapter();
        }

        private async void OnDelete()
        {
            foreach (var selectedGoal in adapter.SelectedGoals)
            {
                repository.Remove(selectedGoal);
                imageService.Remove(selectedGoal.Image);
            }

             await ResetAdapter();
        }

        private async Task ResetAdapter()
        {
            adapter.CancelEditMode();
            adapter.Clear();
            await SetItemsAsync();
            OnEditmodeEnded();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId != Android.Resource.Id.Home) return base.OnOptionsItemSelected(item);
            cancellationTokenSource?.Cancel();
           
            var intent = NavUtils.GetParentActivityIntent(this);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            NavUtils.NavigateUpTo(this, intent);
            return true;
        }

        public override void OnBackPressed()
        {
            cancellationTokenSource?.Cancel();
            base.OnBackPressed();
        }
    }

    internal class SupportActionBarCallback : Java.Lang.Object, ActionMode.ICallback
    {
        private readonly AppCompatActivity activity;
        private readonly Action onDelete;
        private readonly Action onCancel;

        public SupportActionBarCallback(AppCompatActivity activity, Action onDelete, Action onCancel)
        {
            this.activity = activity;
            this.onDelete = onDelete;
            this.onCancel = onCancel;
        }

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            if (item.ItemId == Resource.Id.item_delete)
            {
                onDelete();
            }

            return true;
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            activity.MenuInflater.Inflate(Resource.Menu.cab_delete_menu, menu);

            return true;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            onCancel();
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            mode.Title = "1";
            return true;
        }
    }
}