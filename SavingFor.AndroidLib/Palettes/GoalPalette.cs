using System;
using Android.Graphics;
using Android.Views;
using SavingFor.AndroidLib.Apis;
using Android.Support.V7.Graphics;

namespace SavingFor.AndroidLib.Palettes
{
    class GoalPalett : ISetPalett
    {
        private readonly ISetPalett decorated;

        public GoalPalett(ISetPalett decorated, Bitmap bitmap, int defaultColor)
        {
            this.decorated = decorated;
            var palette = Palette.From(bitmap).Generate();
            var color = GetSwatch(palette, defaultColor);
            var c = new Color(color);
            var red = Color.GetRedComponent(c);
            var green = Color.GetGreenComponent(c);
            var blue = Color.GetBlueComponent(c);

            Color = new Color(red, green, blue, 235);
        }

        private static int GetSwatch(Palette palette, int defaultColor)
        {
            if (palette.DarkVibrantSwatch != null)
                return palette.DarkVibrantSwatch.Rgb;
            return palette.DarkMutedSwatch?.Rgb ?? palette.GetVibrantColor(defaultColor);
        }

        public void SetPalette(View view)
        {
            decorated.Color = Color;
            decorated.SetPalette(view);
        }

        public Color Color { get; set; }
        public void SetRandomPalette(View view, Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public void SetRandomPalette(View view)
        {
            throw new NotImplementedException();
        }
    }
}