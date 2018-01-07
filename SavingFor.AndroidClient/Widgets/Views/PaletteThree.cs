using System;
using Android.Content;
using Android.Graphics;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public sealed class PaletteThree : LineViewBase
    {
        public PaletteThree(Context context):base(context)
        {
            SetZ(3);
        }

        protected override int GetLeft()
        {
            return 0;
        }

        protected override int GetRight()
        {
            return 350;
        }

        protected override int GetRotation()
        {
            return -45;
        }

        protected override int GetTop()
        {
            return 0;
        }

        protected override Color Color => Resources.GetColor(Resource.Color.palette_3, default(Android.Content.Res.Resources.Theme));
    }
}