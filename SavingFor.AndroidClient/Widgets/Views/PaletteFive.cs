using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public sealed class PaletteFive : LineViewBase
    {
        public PaletteFive(Context context):base(context)
        {
            SetZ(1);
        }
        protected override Color Color => new Color(ContextCompat.GetColor(Context, Resource.Color.palette_5));

        protected override int GetLeft()
        {
            return 900;
        }

        protected override int GetRight()
        {
            return 902;
        }

        protected override int GetRotation()
        {
            return 45;
        }

        protected override int GetTop()
        {
            return -1050;
        }
    }
}