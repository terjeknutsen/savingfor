using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Android.Animation;

namespace SavingFor.AndroidClient.Activities.Helpers.Animators
{
    class TitleAnimator
    {
        private CollapsingToolbarLayout collapsingToolbar;

        public TitleAnimator(CollapsingToolbarLayout collapsingToolbar)
        {
            this.collapsingToolbar = collapsingToolbar;
        }

        internal void AnimateShow(string total,Action<string> onEvaluated)
        {
            var title = collapsingToolbar.Title;

            var anim = ObjectAnimator.OfObject(collapsingToolbar, "title", new CharSequenceEvaluator(onEvaluated),
                title, total);

            anim.SetDuration(600);
            anim.Start();
        }

        internal void AnimateHide(string name,Action<string> onEvaluated)
        {
            var title = collapsingToolbar.Title;
            var anim = ObjectAnimator.OfObject(collapsingToolbar, "title", new CharSequenceEvaluator(onEvaluated),
                title, name);
            anim.SetDuration(600);
            anim.Start();
        }
    }
}