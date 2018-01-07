using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using SavingFor.AndroidClient.Activities.Ads;
using SavingFor.AndroidClient.Adapters;
using SavingFor.AndroidClient.Constants;
using SavingFor.AndroidClient.Factories;
using SavingFor.AndroidClient.Settings;
using SavingFor.AndroidClient.Utils;
using SavingFor.LPI;
using SavingFor.Domain.Model;
using SavingFor.Domain.Service;
using Android.Graphics;

namespace SavingFor.AndroidClient.Activities
{
    [Activity(Label = "MonthlyPlanActivity", Theme = "@style/material_theme_modifiable", ParentActivity = typeof(MainActivity), ScreenOrientation = ScreenOrientation.Portrait)]
    public class MonthlyPlanActivity : AppCompatActivity
    {
        private RecyclerView recyclerView;
        private MonthlyPlanAdapter adapter;
        private Toolbar toolbar;
        private readonly IRepository<Goal> repository;
        private readonly Stopwatch timer = new Stopwatch();
        private CancellationTokenSource cancellationtokenSource;


        public MonthlyPlanActivity()
        {
            timer.Start();
            repository = RepositoryFactory.GetSingleton().GetRepository();
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            TrySetStatusbarColor();
            SetContentView(Resource.Layout.activity_statistic_view);
            BindView();
            SetupView();
            SetupToolbar();

            var plan = await CreatePlanAsync(Intent.GetBundleExtra(AppConstant.SelectedGoalsBundle));
            SetPlan(plan);

            await LoadAdsAsync();
        }

        private void TrySetStatusbarColor()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                Window.SetStatusBarColor(Resources.GetColor(Resource.Color.primary_dark,default(Android.Content.Res.Resources.Theme)));
        }

        private async Task LoadAdsAsync()
        {
            cancellationtokenSource = new CancellationTokenSource();
            var adView = FindViewById<AdView>(Resource.Id.adView_monthly_plan);
            var loader = new AsyncLoader(adView,TaskScheduler.FromCurrentSynchronizationContext());
            await loader.LoadAdViewAsync(cancellationtokenSource.Token);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId != Android.Resource.Id.Home) return base.OnOptionsItemSelected(item);
            cancellationtokenSource?.Cancel();
            var intent = NavUtils.GetParentActivityIntent(this);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            NavUtils.NavigateUpTo(this, intent);
            return true;
        }

        public override void OnBackPressed()
        {
            cancellationtokenSource?.Cancel();
            base.OnBackPressed();
        }

        private void BindView()
        {
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_monthly_plan);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_monthly_plan);
        }

        private void SetupView()
        {
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            adapter = new MonthlyPlanAdapter(Assets);
            recyclerView.SetAdapter(adapter);
        }

        private void SetupToolbar()
        {
            if (toolbar == null) return;

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetTitle(Resource.String.title_monthly_plan);
            var color = Resources.GetColor(Resource.Color.primary, default(Android.Content.Res.Resources.Theme));
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(color));
        }

        private async Task<IEnumerable<MonthlyPlan>>  CreatePlanAsync(Bundle bundle)
        {
            var allGoals = await repository.AllAsync();
            if (bundle != null)
            {
                var selected = bundle.GetStringArray(AppConstant.SelectedGoals);
                if(selected.Any())
                allGoals = allGoals.Where(g => bundle.GetStringArray(AppConstant.SelectedGoals).Contains(g.Id.ToString()));
            }
            var planFactory = new PlanFactory(allGoals, new DebugLogger());
            return planFactory.CreatePlan();
        }

        private void SetPlan(IEnumerable<MonthlyPlan> plan)
        {
            
           RunOnUiThread(() =>
           {
               var monthlyPlans = plan as MonthlyPlan[] ?? plan.ToArray();
               adapter.SetPlans(monthlyPlans.ToList());

               if (monthlyPlans.Any())
               {
                   FindViewById<CoordinatorLayout>(Resource.Id.coordinator_monthly_plan).SetBackgroundResource(Resource.Color.window_background);
               }
               else
               {
                   FindViewById<CoordinatorLayout>(Resource.Id.coordinator_monthly_plan).SetBackgroundResource(Resource.Drawable.background);
               }
           }); 

            
        }
    }

    class DebugLogger : ILogger
    {
        public void Log(string text)
        {
            Android.Util.Log.Debug("PLAN", text);
        }
    }
}