using System;
using Android.Views;
using Android.App;

namespace SavingFor.AndroidClient.Activities
{
    internal class MyViewTreeObserver : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
    {
        private ViewTreeObserver observer;
        private Activity activity;
        public MyViewTreeObserver(Activity activity,ViewTreeObserver observer)
        {
            this.activity = activity;
            this.observer = observer;
        }
        public bool OnPreDraw()
        {
            observer.RemoveOnPreDrawListener(this);
            activity.StartPostponedEnterTransition();
            return true;
        }
    }
}