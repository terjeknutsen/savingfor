using Android.Views;
using Android.Animation;

namespace SavingFor.AndroidClient.Activities.Helpers.Animators
{
    class AccountFabAnimator
    {
        View view;
        public AccountFabAnimator(View view)
        {
            this.view = view;
        }
        public void AnimateHide()
        {
            if (view.Visibility == ViewStates.Visible)
            {
                var accountfabAnimation = ObjectAnimator.OfFloat(view, "alpha", 1f, 0f);
                accountfabAnimation.SetDuration(60);
                accountfabAnimation.Start();
                view.Enabled = false;
            }
        }

        internal void AnimateShow()
        {
            if (view.Visibility == ViewStates.Visible)
            {
                var accountfabAnimation = ObjectAnimator.OfFloat(view, "alpha", 0f, 1f);
                accountfabAnimation.SetDuration(600);
                accountfabAnimation.Start();
                view.Enabled = true;
            }
        }
    }
}