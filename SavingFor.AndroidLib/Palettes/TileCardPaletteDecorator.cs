using System;
using Android.Graphics;
using Android.Support.V7.Graphics;
using Android.Util;
using Android.Views;
using SavingFor.AndroidLib.Apis;
using Android.Support.V7.Widget;

namespace SavingFor.AndroidLib.Palettes
{
    public class TileCardPaletteDecorator : ISetPalett
    {
        private readonly ISetPalett decorated;

        public TileCardPaletteDecorator(ISetPalett decorated, Bitmap bitmap)
        {
            this.decorated = decorated;
            var palette = Palette.From(bitmap).Generate();

            try
            {
                //Color = new Color(palette.DarkMutedSwatch.Rgb);
                Color = new Color(palette.GetLightMutedColor(Resource.Color.cardview_light_background));
            }
            catch (Exception)
            {
                Log.Debug("GOAL", "TilePalette failed unexpectedly");
            }
        }

        public void SetPalette(View view)
        {
            var card = view as CardView;
            if (card == null) return;
            decorated.Color = Color;
            card.SetCardBackgroundColor(Color);
        }

        public Color Color { get; set; }
        public void SetRandomPalette(View view, Bitmap bitmap)
        {
        }
    }
}