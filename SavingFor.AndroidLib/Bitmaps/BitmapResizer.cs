using Android.Graphics;
using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidLib.Bitmaps
{
    public sealed class BitmapResizer : IResizeBitmap
    {

        public Bitmap Resize(Bitmap bitmap, int width, int height, bool rectangular)
        {
            var w = bitmap.Width;
            var h = bitmap.Height;
            var scaledHeight = width * h / w;

            if (rectangular)
            {
                scaledHeight = width;
            }
            return Bitmap.CreateScaledBitmap(bitmap, width, scaledHeight, true);
        }
    }
}