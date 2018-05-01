using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public sealed class ShadowFour : LineViewBase
    {
        public ShadowFour(Context context):base(context)
        {
            SetZ(3);
        }
        protected override Color Color => new Color(ContextCompat.GetColor(Context, Resource.Color.shadow_4));

        protected override int GetLeft()
        {
            return -24;
        }

        protected override int GetRight()
        {
            return 0;
        }

        protected override int GetRotation()
        {
            return -45;
        }

        protected override int GetTop()
        {
            return 0;
        }
    }
}