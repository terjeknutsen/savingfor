using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidLib.Palettes
{
    public class TileTextPaletteDecorator : ISetPalett
    {
        private readonly ISetPalett decorated;

        public TileTextPaletteDecorator(ISetPalett decorated, Bitmap bitmap)
        {
            this.decorated = decorated;
            if (bitmap == null) return;
            var palette = Palette.From(bitmap).Generate();

            try
            {
                Color = new Color(GetFirstSwatch(palette));
            }
            catch (Exception)
            {
                Log.Debug("GOAL", "TilePalette failed unexpectedly");
            }
        }

        private static int GetFirstSwatch(Palette palette)
        {
            return palette?.MutedSwatch?.Rgb ?? palette?.DarkMutedSwatch?.Rgb ?? Color.DarkGoldenrod;
        }

        public void SetPalette(View view)
        {
            decorated.Color = Color;
            decorated.SetPalette(view);
        }

        public Color Color { get; set; }
        public void SetRandomPalette(View view, Bitmap bitmap)
        {
        }
    }
}