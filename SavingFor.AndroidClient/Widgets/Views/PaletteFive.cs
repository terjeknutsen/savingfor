using Android.Content;
using Android.Graphics;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public sealed class PaletteFive : LineViewBase
    {
        public PaletteFive(Context context):base(context)
        {
            SetZ(1);
        }
        protected override Color Color => Resources.GetColor(Resource.Color.palette_5, default(Android.Content.Res.Resources.Theme));

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