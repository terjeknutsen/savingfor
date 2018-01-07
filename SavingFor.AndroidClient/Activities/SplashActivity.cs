using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace SavingFor.AndroidClient.Activities
{
#if TEST
    [Activity(Theme = "@style/SplashTheme",Label = "Spare test", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
#else
    [Activity(Theme = "@style/SplashTheme",MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
#endif
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var intent = new Intent(this, typeof(MainActivity));
           
            StartActivity(intent);
            Finish();
    }
    }
}