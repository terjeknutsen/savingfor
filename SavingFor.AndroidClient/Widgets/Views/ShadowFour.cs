using Android.Content;
using Android.Graphics;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public sealed class ShadowFour : LineViewBase
    {
        public ShadowFour(Context context):base(context)
        {
            SetZ(3);
        }
        protected override Color Color => Resources.GetColor(Resource.Color.shadow_4, default(Android.Content.Res.Resources.Theme));

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