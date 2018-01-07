using System;
using Android.Content;
using Android.Graphics;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public sealed class PaletteTwo : LineViewBase
    {
        public PaletteTwo(Context context):base(context)
        {
            SetZ(4);
        }

        protected override int GetLeft()
        {
            return 200;
        }

        protected override int GetRight()
        {
            return 250;
        }


        protected override int GetRotation()
        {
            return 45;
        }

        protected override int GetTop()
        {
            return -900;
        }

        protected override Color Color => Resources.GetColor(Resource.Color.palette_2,default(Android.Content.Res.Resources.Theme));
    }
}