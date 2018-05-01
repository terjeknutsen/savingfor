using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using SavingFor.AndroidClient.Factories;
using SavingFor.AndroidClient.Settings;
using SavingFor.AndroidLib;
using SavingFor.AndroidLib.Apis;
using SavingFor.Domain.Model;
using SavingFor.LPI;
using Fragment = Android.Support.V4.App.Fragment;
using SavingFor.AndroidLib.Palettes;
using Android.Support.V4.Content;

namespace SavingFor.AndroidClient.Fragments
{
    public class GoalDetailsFragment : Fragment
    {
        private IRepository<Goal> repositoryAndroid;
        private IImageService imageService;
        private ImageView firstImageView;
        private IDictionary<Guid,Goal> images = new Dictionary<Guid, Goal>();
        private ImageView secondImageView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            repositoryAndroid = RepositoryFactory.GetSingleton().GetRepository();
            imageService = SimpleIoC.GetImageService(Activity, Resources.DisplayMetrics);
            var view = inflater.Inflate(Resource.Layout.fragment_next_goal_view, container, false);
            BindView(view);
            SetImages();
            return view;
        }

        

        private void SetImages()
        {
            images = repositoryAndroid.All().Where(g => g.End > DateTime.Now).ToDictionary(g => g.Id);
        }

        public async Task<Guid> SetDetails(Guid goalId)
        {
            if (images.ContainsKey(goalId))
               await SetImageAsync(images[goalId].Image);
            else
            {
                var availableIds = images.Keys.ToList();
                if (availableIds.Any())
                    await SetImageAsync(images[availableIds[new Random().Next(0, images.Count)]].Image);
                else
                {
                    var primary = ContextCompat.GetColor(Activity, Resource.Color.primary);
                    var statusBarScrim = new Color(ContextCompat.GetColor(Activity,Resource.Color.primary_dark));
                    var contentScrim = new Color(ContextCompat.GetColor(Activity, Resource.Color.primary));
                    var background = new Color(ContextCompat.GetColor(Activity,Resource.Color.window_background));
                    Preferences.CurrentColor = new Color(ContextCompat.GetColor(Activity, Resource.Color.primary));
                    Preferences.HeroImageGoalId = Guid.Empty.ToString();
                    CollapsingToolbar.ContentScrim = new ColorDrawable(contentScrim);
                    CollapsingToolbar.Background = new ColorDrawable(background);
                    CollapsingToolbar.StatusBarScrim = new ColorDrawable(statusBarScrim);
                }
            }
            return goalId;
        }

        private string current = string.Empty;
        public CollapsingToolbarLayout CollapsingToolbar { get; set; }

        private async Task SetImageAsync(string image)
        {
            if (!Preferences.IsDefaultColor(Preferences.CurrentColor))
            {
                var color = Preferences.CurrentColor;
                CollapsingToolbar.ContentScrim = new ColorDrawable(color);
                CollapsingToolbar.Background = new ColorDrawable(color);
                CollapsingToolbar.StatusBarScrim = new ColorDrawable(color);
            }

            if (image == current) return;

            var bitmap = await imageService.GetBitmapAsync(image);
  
            Activity.RunOnUiThread(() =>
            {
                var color = Color.Black;
                var tmp = imageService.Blur(image, bitmap,color);
                Crossfade(tmp, firstImageView.Visibility == ViewStates.Gone);
            });
                
            current = image;
        }

        private void Crossfade(Bitmap bitmap, bool shouldFadeInFirstImage)
        {
            if (shouldFadeInFirstImage)
            {
                Crossfade(bitmap,firstImageView,secondImageView);
            }
            else
            {
                Crossfade(bitmap,secondImageView,firstImageView);
            }
        }

        private static void Crossfade(Bitmap bitmap,ImageView fadeIn,ImageView fadeOut)
        {
            fadeIn.Alpha = 0f;

            fadeIn.SetImageBitmap(bitmap);
            fadeIn.Visibility = ViewStates.Visible;

            fadeOut.Animate().Alpha(0f).SetDuration(1200).SetInterpolator(new AccelerateInterpolator()).SetListener(new AnimationListener(fadeOut));
            fadeIn.Animate()
                .Alpha(1f)
                .SetDuration(1200).SetInterpolator(new AccelerateDecelerateInterpolator())
                .SetListener(null);
        }


        private void BindView(View view)
        {
            firstImageView = view.FindViewById<ImageView>(Resource.Id.image_view_goal_first);
            secondImageView = view.FindViewById<ImageView>(Resource.Id.image_view_goal_second);
        }

        public void Clear()
        {
           firstImageView.SetImageBitmap(null);
           secondImageView.SetImageBitmap(null); 
        }

        public void Refresh()
        {
            Clear();
            SetImages();
            current = string.Empty;
        }
    }

    internal class AnimationListener : Java.Lang.Object, Animator.IAnimatorListener
    {
        private readonly ImageView fadeOut;

        public AnimationListener(ImageView fadeOut)
        {
            this.fadeOut = fadeOut;
        }

        public void OnAnimationCancel(Animator animation)
        {
        }

        public void OnAnimationEnd(Animator animation)
        {
            fadeOut.Visibility = ViewStates.Gone;
        }

        public void OnAnimationRepeat(Animator animation)
        {
        }

        public void OnAnimationStart(Animator animation)
        {
        }
    }
}