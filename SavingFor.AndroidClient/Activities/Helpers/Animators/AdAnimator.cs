using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Animation;

namespace SavingFor.AndroidClient.Activities.Helpers.Animators
{
    class AdAnimator
    {
        private AdView adView;

        public AdAnimator(AdView adView)
        {
            this.adView = adView;
        }

        internal void AnimateHide()
        {
            var adAnimation = ObjectAnimator.OfFloat(adView, "alpha", 1f, 0f);
            adAnimation.SetDuration(600);
            adAnimation.Start();
            adView.Visibility = ViewStates.Gone;
        }

        internal void AnimateShow()
        {
            var adAnimation = ObjectAnimator.OfFloat(adView, "alpha", 0f, 1f);
            adAnimation.SetDuration(600);
            adAnimation.Start();
            adView.Visibility = ViewStates.Visible;
        }
    }
}