using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using SavingFor.AndroidClient.Adapters;
using SavingFor.AndroidClient.Constants;
using SavingFor.AndroidClient.Extensions;
using SavingFor.AndroidClient.Factories;
using SavingFor.AndroidClient.Fragments;
using SavingFor.AndroidClient.Settings;
using SavingFor.AndroidLib;
using SavingFor.Domain.Model;
using SavingFor.LPI;
using SavingFor.Services;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Math = System.Math;
using Object = Java.Lang.Object;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.Util;
using System.Security;
using System.Threading;
using Android.Gms.Ads;
using SavingFor.AndroidClient.Activities.Ads;
using System.Collections.Generic;
using SavingFor.AndroidClient.Activities.Helpers;
using SavingFor.AndroidClient.Activities.Helpers.Animators;
using SavingFor.AndroidClient.Services;
using SavingFor.AndroidClient.Broadcasts;
using Android.Support.V4.Content;
using SavingFor.AndroidClient.Interfaces;
using SavingFor.AndroidClient.Dialogs;
using Android.Widget;

namespace SavingFor.AndroidClient.Activities
{
    [Activity(Theme = "@style/material_theme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, 
        IHandleGoal,
        ILinkGoalGroup,
        IDeleteGoal,
        IAddGroup
    {
        private Toolbar toolbar;
        private FloatingActionButton fab;
        private RecyclerView recyclerView;
        private GoalItemAdapter adapter;
        private ActionMode mActionMode;
        public const string GoalUpdated = "com.track.goal.goal_updated";

        public const string GoalAdded = "com.track.goal.goal_added";

        private GoalDetailsFragment goalDetails;
        private LineAnimationFragment emptyCollection;
        private CoordinatorLayout rootLayout;

        private TotalUpdater totalUpdater;
        private bool shouldStartUpdaterOnResume;
        private CollapsingToolbarLayout collapsingToolbar;
        private AppBarLayout appbarLayout;
        private bool isCollapsed;
        private bool isInAnimation;
        private IRepository<Goal> repository;
        private AdView adView;
        private FloatingActionButton accountFab;
        private AccountHelper accountHelper;
        private AccountFabAnimator accountFabAnimator;
        private AdAnimator adAnimator;
        private TitleAnimator titleAnimator;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Window.AddFlags(WindowManagerFlags.TranslucentStatus);
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            totalUpdater = new TotalUpdater(OnTotalUpdate);
            SetContentView(Resource.Layout.activity_main_view);
            goalDetails = SupportFragmentManager.FindFragmentById(Resource.Id.fragment_next_goal) as GoalDetailsFragment;
            if (goalDetails != null)
                goalDetails.CollapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar_layout);

            emptyCollection =
                SupportFragmentManager.FindFragmentById(Resource.Id.fragment_empty_collection) as
                    LineAnimationFragment;
            accountHelper = new AccountHelper(PackageManager,Resources, intent => StartActivity(intent));
            BindView();
            accountFabAnimator = new AccountFabAnimator(accountFab);
            
            TrySetToolbar();
            SetupFabClickHandling();
            await SetPreferredImageAsync();
            await SetupRecyclerView();
            await InitializeAsync().ContinueWith(_ =>
            {
                RestartTotalUpdate();
                shouldStartUpdaterOnResume = true;
            });
            appbarLayout.AddOnOffsetChangedListener(new OffsetChangedListener(OnCollapsed,OnExpanded));
            LoadAds();
            adAnimator = new AdAnimator(adView);
            titleAnimator = new TitleAnimator(collapsingToolbar);
        }

        private void LoadAds()
        {
            adView = FindViewById<AdView>(Resource.Id.adView_main);
            var loader = new AsyncLoader(adView, TaskScheduler.FromCurrentSynchronizationContext());
            loader.LoadAdViewAsync(CancellationToken.None);
        }

        private void OnCollapsed(int verticalOffset)
        {

            if (appbarLayout.TotalScrollRange + verticalOffset == 0)
                toolbar.SetBackgroundColor(new Color(ContextCompat.GetColor(this,Resource.Color.primary)));
            else toolbar.SetBackgroundColor(Color.Transparent);
            if (isCollapsed)
            {
                return;
            }
            isInAnimation = true;
            titleAnimator.AnimateHide(Resources.GetString(Resource.String.application_name),OnEvaluated);
            accountFabAnimator.AnimateHide();
            adAnimator.AnimateHide();
            isCollapsed = true;
            isInAnimation = false;
        }
        [SecuritySafeCritical]
        private void OnExpanded(int verticalOffset)
        {
            if (!isCollapsed)
            {
                fab.Visibility = ViewStates.Visible;
                return;
            }
            isInAnimation = true;
            titleAnimator.AnimateShow(totalUpdater.Value.ToString("C"), OnEvaluated);
            accountFabAnimator.AnimateShow();
            adAnimator.AnimateShow();
            isCollapsed = false;
            isInAnimation = false;
        }

        private void OnEvaluated(string evalueated)
        {
            collapsingToolbar.Title = evalueated;
        }

        private void OnTotalUpdate(decimal @decimal)
        {
            
            RunOnUiThread(() =>
            {
                if(!isInAnimation)
                collapsingToolbar.Title = isCollapsed ? Resources.GetString(Resource.String.application_name) : @decimal.ToString("C");
                adapter.OnRefresh();
            });

        }
        [SecuritySafeCritical]
        protected override void OnResume()
        {
            base.OnResume();
            groupCreatedReceiver = new GroupCreatedReceiver(this);
            groupLinkRemoved = new GroupLinkRemovedReceiver(this);
            LocalBroadcastManager.GetInstance(this).RegisterReceiver(groupCreatedReceiver, new IntentFilter(nameof(GroupCreatedReceiver)));
            LocalBroadcastManager.GetInstance(this).RegisterReceiver(groupLinkRemoved, new IntentFilter(nameof(GroupLinkRemovedReceiver)));
            if (shouldStartUpdaterOnResume)
            totalUpdater.Start();
            if (adapter != null && adapter.ItemCount == 0)
            {
                ShowEmptyCollectionView();
            }
        }

       
        [SecuritySafeCritical]
        protected override void OnPause()
        {
            base.OnPause();
            LocalBroadcastManager.GetInstance(this).UnregisterReceiver(groupCreatedReceiver);
            LocalBroadcastManager.GetInstance(this).UnregisterReceiver(groupLinkRemoved);
            totalUpdater.Stop();
        }
        [SecuritySafeCritical]
        protected override void OnDestroy()
        {
            base.OnDestroy();
            totalUpdater.Dispose();
        }


        protected override async void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            shouldStartUpdaterOnResume = false;
            if (intent.GetBooleanExtra(IntentExtra.GoalAdded, false))
            {
                goalDetails.Refresh();
                adapter.Clear();
                await InitializeAsync();
            }

            await SetPreferredImageAsync();
            
            RestartTotalUpdate();
            shouldStartUpdaterOnResume = true;
        }
        [SecuritySafeCritical]
        private void RestartTotalUpdate()
        {
            totalUpdater.Set(adapter.Items.Select(i => new GoalAmountRequired(i.ProgressPerMinute, i.Start)));
            totalUpdater.Start();
        }

        private async Task SetPreferredImageAsync()
        {
            Guid guid;
            var parsed = Guid.TryParse(Preferences.HeroImageGoalId, out guid);
            if (!parsed)
                guid = Guid.NewGuid();
            await goalDetails.SetDetails(guid);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            GC.Collect();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            adapter.CancelEditMode();
            emptyCollection.StopAnimation();
            var intent = item.ItemId == Resource.Id.item_statistics ?
                new Intent(this, typeof(MonthlyPlanActivity)) :
                new Intent(this, typeof(FinishedGoalsActivity));

            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.abc_fade_in,Resource.Animation.abc_fade_out);
            return true;
        }

        private void BindView()
        {
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab_button);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view_main);
            rootLayout = FindViewById<CoordinatorLayout>(Resource.Id.coordinator_main);
            appbarLayout = FindViewById<AppBarLayout>(Resource.Id.app_bar_layout_main);
            accountFab = FindViewById<FloatingActionButton>(Resource.Id.fab_account);
            if (accountHelper.CanShowAccount)
                accountFab.Visibility = ViewStates.Visible;
        }

        private void TrySetToolbar()
        {
            if (toolbar == null) return;

            SetSupportActionBar(toolbar);
            collapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar_layout);
            collapsingToolbar.CollapsedTitleTypeface = Roboto.Medium(Assets);
            collapsingToolbar.ExpandedTitleTypeface = Roboto.LightItalic(Assets);
        }

        private async Task SetupRecyclerView()
        {
            var imageService = SimpleIoC.GetImageService(this, Resources.DisplayMetrics);
            var placeholder =
                await
                    Task<Bitmap>.Factory.StartNew(
                        () => BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_broken_image_white_48dp));
            adapter = new GoalItemAdapter(ContentResolver, Roboto.Medium(Assets), placeholder, Resources, imageService);
            recyclerView.SetLayoutManager(new GridLayoutManager(this, 2, LinearLayoutManager.Vertical, false));
            recyclerView.SetAdapter(adapter);
            recyclerView.HasFixedSize = true;
            adapter.ItemClick += OnItemClick;
            adapter.ItemLongClick += OnItemLongClick;
            adapter.ModeChanged += OnModeChanged;
            adapter.ItemCheckChanged += OnItemCheckChanged;

        }

        private void SetupFabClickHandling()
        {
            fab.Click += OnFabClicked;
            accountFab.Click += OnAccountFabClicked;
        }

        private void OnAccountFabClicked(object sender, EventArgs e)
        {
            
            if (accountHelper.CanShowAccount)
                accountHelper.ShowAccount();
        }
        static List<string> availableBankApps = new List<string> { "no.sparebank1.mobilbank" };
        private GroupCreatedReceiver groupCreatedReceiver;
        private GroupLinkRemovedReceiver groupLinkRemoved;

        private async Task InitializeAsync()
        {
            var allGoals = await GetRepository().AllAsync();
            adapter.Clear();
            var currentGroup = Preferences.CurrentGroup;
            RunOnUiThread(() =>
            {
                var displayGoals = allGoals.Where(g => g.End > DateTime.Now);
                if (!string.IsNullOrEmpty(currentGroup))
                {
                    displayGoals = displayGoals.Where(g => g.Group == currentGroup);
                }
               
                adapter.SetGoals(displayGoals);

            });

            if (adapter.ItemCount > 0)
            {
                emptyCollection.StopAnimation();
                goalDetails.View.Visibility = ViewStates.Visible;
            }
            else
            {
                goalDetails.View.Visibility = ViewStates.Gone;
                emptyCollection.StartAnimation();
                rootLayout.SetBackgroundResource(Resource.Drawable.background);
                return;
            }

            rootLayout.Background = null;
        }

        [SecuritySafeCritical]
        private IRepository<Goal> GetRepository()
        {
            return repository ?? (repository = RepositoryFactory.GetSingleton().GetRepository());
        }

        private void OnItemCheckChanged(object sender, ItemCheckChangedEvent e)
        {
            mActionMode.Title = adapter.CheckedItems.ToString("G");
        }

        void OnItemClick(object sender, GoalClickedEvent @event)
        {
            var intent = new Intent(this, typeof(GoalEditActivity));
            intent.PutExtra(GoalEditActivity.GoalId, @event.Id.ToString());
            intent.PutExtra(GoalEditActivity.ColorPalette, @event.Color);
            var imagePair = new Pair(@event.ImageView, GetString(Resource.String.transition_goal_image));
            var fabButtonPair = new Pair(fab, "fab_button");

            var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this,imagePair,fabButtonPair);

            ActivityCompat.StartActivity(this,intent,options.ToBundle());
        }

        private void OnItemLongClick(object sender, int e)
        {
            mActionMode = StartActionMode(new DefaultActionBarCallback(this,toolbar,this,this,this));
            //mActionMode = StartActionMode(
            //    new ActionBarCallback(this, 
            //    OnDelete, 
            //    OnCancel,
            //    OnShowMonthlyPlanForSelectedItems, 
            //    OnLink,
            //    OnUnlink));
        }

        private void OnUnlink()
        {
            var selectedItems = adapter.SelectedGoals;
            var intent = new Intent(this, typeof(GoalUnlinkService));
            var bundle = new Bundle();
            bundle.PutStringArray(nameof(Goal.Id), selectedItems.Select(g => g.Id.ToString()).ToArray());
            intent.PutExtra(nameof(Goal), bundle);

            StartService(intent);
        }
        private void OnLink()
        {
            var allGroups = Preferences.UsedGroups.ToArray();
            var bundle = new Bundle();
            bundle.PutStringArray(nameof(Goal.Group), allGroups);
            var linkDialog = LinkDialog.NewInstance(bundle);
            linkDialog.Show(SupportFragmentManager,"link");
       
        }
        private void OnShowMonthlyPlanForSelectedItems()
        {
            shouldStartUpdaterOnResume = false;
            var intent = new Intent(this,typeof(MonthlyPlanActivity));
            var bundle = new Bundle();
            bundle.PutStringArray(AppConstant.SelectedGoals,adapter.SelectedGoals.Select(g => g.Id.ToString()).ToArray());
            intent.PutExtra(AppConstant.SelectedGoalsBundle, bundle);

            StartActivityForResult(intent,3952);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 3952)
            {
                adapter.RefreshView();
            }
            shouldStartUpdaterOnResume = true;
        }

        private void OnCancel()
        {
            adapter.CancelEditMode();
        }

        private void OnDelete()
        {
            var dialog = new AlertDialog.Builder(this)
                .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                .SetTitle(Resource.String.delete_confirmation)
                .SetPositiveButton(Resource.String.delete_positive,delegate {Delete();})
                .SetNegativeButton(Resource.String.delete_negative,delegate {});
            dialog.Show();
        }
        [SecuritySafeCritical]
        private void Delete()
        {
            var index = Preferences.HeroImageGoalId;
            if (adapter.IsToBeDeleted(new Guid(index)))
                Preferences.HeroImageGoalId = Guid.Empty.ToString();

            foreach (var selectedGoal in adapter.SelectedGoals)
            {
                RepositoryFactory.GetSingleton().GetRepository().Remove(selectedGoal);
            }
            adapter.RemoveSelectedGoals();
            adapter.CancelEditMode();
            totalUpdater.Stop();

            goalDetails.Refresh();
            if (adapter.ItemCount == 0)
            {
                goalDetails.View.Visibility = ViewStates.Gone;
                ShowEmptyCollectionView();
                collapsingToolbar.Title = 0.0m.ToString("C");
                totalUpdater.Reset();
                return;
            }
            else
            {
                goalDetails.View.Visibility = ViewStates.Visible;
                RunOnUiThread(async () => await goalDetails.SetDetails(new Guid(Preferences.HeroImageGoalId)));
                RestartTotalUpdate();
            }

            
        }

        private void OnModeChanged(object sender, bool anyChecked)
        {
            if (!anyChecked)
                mActionMode?.Finish();
        }

        private void OnFabClicked(object sender, EventArgs e)
        {
            adapter.CancelEditMode();
            var intent = new Intent(this, typeof(GoalEditActivity));
            var fabButtonPair = new Pair(fab, "fab_button");
            var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, fabButtonPair);

            intent.PutExtra(GoalEditActivity.NewGoal, true);
            
            ActivityCompat.StartActivity(this, intent, options.ToBundle());
        }

        private void ShowEmptyCollectionView()
        {
            emptyCollection.StartAnimation();
            rootLayout.SetBackgroundResource(Resource.Drawable.background);
        }

        public void GoalGroupSelected(string groupName)
        {
            Preferences.HeroImageGoalId = Guid.Empty.ToString();

            adapter.CancelEditMode();
            adapter.Clear();
            adapter.SetGoals(repository.All().Where(g => g.End > DateTime.Now && g.Group == groupName));
            totalUpdater.Stop();

            goalDetails.Refresh();
            if (adapter.ItemCount == 0)
            {
                goalDetails.View.Visibility = ViewStates.Gone;
                ShowEmptyCollectionView();
                collapsingToolbar.Title = 0.0m.ToString("C");
                totalUpdater.Reset();
                return;
            }
            else
            {
                goalDetails.View.Visibility = ViewStates.Visible;
                RunOnUiThread(async () => await goalDetails.SetDetails(new Guid(Preferences.HeroImageGoalId)));
                RestartTotalUpdate();
            }
        }

        public void HandleGroupLinkRemoved()
        {
            Preferences.HeroImageGoalId = Guid.Empty.ToString();

            adapter.CancelEditMode();
            adapter.Clear();
            adapter.SetGoals(repository.All().Where(g => g.End > DateTime.Now));
            totalUpdater.Stop();

            goalDetails.Refresh();
            if (adapter.ItemCount == 0)
            {
                goalDetails.View.Visibility = ViewStates.Gone;
                ShowEmptyCollectionView();
                collapsingToolbar.Title = 0.0m.ToString("C");
                totalUpdater.Reset();
                return;
            }
            else
            {
                goalDetails.View.Visibility = ViewStates.Visible;
                RunOnUiThread(async () => await goalDetails.SetDetails(new Guid(Preferences.HeroImageGoalId)));
                RestartTotalUpdate();
            }

        }

        public void LinkGroup(string text)
        {
            var selectedItems = adapter.SelectedGoals;
            var intent = new Intent(this, typeof(GoalLinkService));
            var bundle = new Bundle();
            bundle.PutStringArray(nameof(Goal.Id), selectedItems.Select(g => g.Id.ToString()).ToArray());
            bundle.PutString(nameof(Goal.Group), text);
            bundle.PutStringArray(nameof(Preferences), Preferences.UsedGroups.ToArray());
            intent.PutExtra(nameof(Goal), bundle);

            StartService(intent);
        }

         public void DeleteGoal()
        {
            var dialog = new AlertDialog.Builder(this)
             .SetIcon(Android.Resource.Drawable.IcDialogAlert)
             .SetTitle(Resource.String.delete_confirmation)
             .SetPositiveButton(Resource.String.delete_positive, delegate { Delete(); })
             .SetNegativeButton(Resource.String.delete_negative, delegate { });
            dialog.Show();
        }

        public void AddGroup()
        {
            var allGroups = Preferences.UsedGroups.ToArray();
            var bundle = new Bundle();
            bundle.PutStringArray(nameof(Goal.Group), allGroups);
            var linkDialog = LinkDialog.NewInstance(bundle);
            linkDialog.Show(SupportFragmentManager, "link");
        }
    }

    class CharSequenceEvaluator : Object, ITypeEvaluator
    {
        private readonly Action<string> onEvaluation;

        public CharSequenceEvaluator(Action<string> onEvaluation)
        {
            this.onEvaluation = onEvaluation;
        }

        public Object Evaluate(float fraction, Object startValue, Object endValue)
        {
            
            var initialTextLenght = startValue == null ? 0 : ((string)startValue).Length;
            var finalTextLenght = endValue == null ? 0 : ((string) endValue).Length;

            if (initialTextLenght == 0 && finalTextLenght == 0)
            {
                onEvaluation((string)endValue);
                return endValue;
            }
            if (fraction <= 0)
            {
                onEvaluation((string) startValue);
                return startValue;
            }
            if (fraction >= 1f)
            {
                onEvaluation((string) endValue);
                return endValue;
            }

            var maxLength = Math.Max(initialTextLenght, finalTextLenght);
            var charactersChanged = (int) (maxLength*fraction);
            if (charactersChanged == 0)
            {
                onEvaluation((string) startValue);
                return startValue;
            }
            if(finalTextLenght < charactersChanged)
            {
                if (finalTextLenght == 0)
                {
                    onEvaluation(((string) startValue).Substring(charactersChanged, initialTextLenght));
                    return ((string) startValue).Substring(charactersChanged, initialTextLenght);
                }
                if (initialTextLenght <= charactersChanged)
                {
                    onEvaluation(((string) endValue).Substring(0, charactersChanged));
                    return ((string) endValue).Substring(0, charactersChanged);
                }
                onEvaluation(((string) endValue) +
                             ((string) startValue).Substring(charactersChanged, initialTextLenght - charactersChanged));
                return ((string) endValue) + ((string) startValue).Substring(charactersChanged, initialTextLenght-charactersChanged);
            }
            if (initialTextLenght <= charactersChanged)
            {
                onEvaluation(((string) endValue).Substring(0, charactersChanged));
                return ((string) endValue).Substring(0, charactersChanged);
            }
            onEvaluation(((string) endValue).Substring(0, charactersChanged) +
                         ((string) startValue).Substring(charactersChanged, initialTextLenght - charactersChanged));
            return ((string) endValue).Substring(0, charactersChanged) +
                   ((string) startValue).Substring(charactersChanged, initialTextLenght-charactersChanged);

        }
    }
}