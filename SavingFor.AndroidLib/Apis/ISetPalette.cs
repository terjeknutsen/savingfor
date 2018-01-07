using Android.Graphics;
using Android.Views;

namespace SavingFor.AndroidLib.Apis
{
    public interface ISetPalett
    {
        void SetPalette(View view);
        Color Color { get; set; }
        void SetRandomPalette(View view, Bitmap bitmap);
    }
}