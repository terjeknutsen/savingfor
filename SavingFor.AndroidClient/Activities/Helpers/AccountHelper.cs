using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;

namespace SavingFor.AndroidClient.Activities.Helpers
{
    class AccountHelper
    {
        private PackageManager packageManager;
        Action<Intent> startActivity;
        Resources resources;

        public AccountHelper(PackageManager packageManager,Resources resources,Action<Intent> startActivity)
        {
            this.packageManager = packageManager;
            this.startActivity = startActivity;
            this.resources = resources;
        }

        public bool CanShowAccount
        {
            get
            {
                if (launchIntents == null)
                    InitializeLaunchIntents();
                return launchIntents.Any();
            }
        }
        IDictionary<string, Intent> launchIntents;
        private void InitializeLaunchIntents()
        {
            var installedPackages = packageManager.GetInstalledApplications(PackageInfoFlags.MetaData);
            launchIntents = new Dictionary<string, Intent>();
            TypedArray supportedPackages;
            try
            {
                supportedPackages = resources.ObtainTypedArray(Resource.Array.supportedPackages);
            }
            catch (Exception)
            {
                return;
            }
            foreach (var info in installedPackages)
            {
                for(int i = 0; i<supportedPackages.Length();i++)
                {
                    var package = supportedPackages.GetString(i);
                    if(package == info.PackageName)
                        launchIntents.Add(info.PackageName, packageManager.GetLaunchIntentForPackage(info.PackageName));
                }
            }
        }

        internal void ShowAccount()
        {
            if (launchIntents.Any())
            {
               startActivity(launchIntents.Values.First());
            }
        }
    }
}