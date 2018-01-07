using Android.Graphics;
using Android.Views;
using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidLib.Palettes
{
    public class PalettUtil : ISetPalett
    {
        public void SetPalette(View view)
        {
            view.SetBackgroundColor(Color);
        }

        public Color Color { get; set; }
        public void SetRandomPalette(View view, Bitmap bitmap)
        {
           
        }

        public void SetRandomPalette(View view)
        {
            SetPalette(view);
        }
    }
}