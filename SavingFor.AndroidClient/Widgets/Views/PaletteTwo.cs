using System;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;

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

        protected override Color Color => new Color(ContextCompat.GetColor(Context,Resource.Color.palette_2)); 
    }
}