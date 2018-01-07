using Android.Content;
using Android.Graphics;
using Android.Util;
using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidLib.Bitmaps
{
    public sealed class BitmapLoader : ILoadBitmap
    {
        public Bitmap Load(Android.Net.Uri uri, ContentResolver contentResolver, DisplayMetrics displayMetrics)
        {
            var input = contentResolver.OpenInputStream(uri);
            var maxWidth = (int)displayMetrics.Xdpi;
            var maxHeight = (int)displayMetrics.Ydpi;
            var options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeStream(input, null, options);
            input.Close();
            options.InSampleSize = CalculateInSampleSize(options, maxWidth, maxHeight);
            options.InJustDecodeBounds = false;
            input = contentResolver.OpenInputStream(uri);
            var bitmap = BitmapFactory.DecodeStream(input, null, options);
            input.Close();
            return bitmap;
        }

        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            var height = options.OutHeight;
            var width = options.OutWidth;
            var samplesize = 1;

            if (height <= reqHeight && width <= reqWidth) return samplesize;

            var halfHeight = height / 2;
            var halfWidth = width / 2;

            while ((halfHeight / samplesize) > reqHeight && (halfWidth / samplesize) > reqWidth)
            {
                samplesize *= 2;
            }

            return samplesize;

        }
    }
}