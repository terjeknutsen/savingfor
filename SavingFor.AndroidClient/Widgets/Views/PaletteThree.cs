using System;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;

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

        protected override Color Color => new Color(ContextCompat.GetColor(Context, Resource.Color.palette_3));
    }
}