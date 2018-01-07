using Android.Content;
using Android.Graphics;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public sealed class PaletteFour : LineViewBase
    {
        public PaletteFour(Context context):base(context)
        {
            SetZ(2);
        }
        protected override Color Color => Resources.GetColor(Resource.Color.palette_4, default(Android.Content.Res.Resources.Theme));

        protected override int GetLeft()
        {
            return -20;
        }

        protected override int GetRight()
        {
            return 95;
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