using Android.Util;
using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidLib.Utils
{
    public struct AppBarImageSize : ISetImageSize
    {
        public AppBarImageSize(DisplayMetrics metrics)
        {
            var tmp = metrics.WidthPixels;
            Width = tmp;
            Height = Width;
        }

        public int Width { get; }
        public int Height { get; }
    }
}