using System;
using System.IO;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using SavingFor.AndroidLib.Apis;
using Uri = Android.Net.Uri;

namespace SavingFor.AndroidLib.Utils
{
    public class BitmapDecoder: Java.Lang.Object
    {
        public static Bitmap DecodeSampledBitmapFromResource(Resources res, int resId, ISetImageSize imageSizer)
        {
            var options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeResource(res, resId, options);

            options.InSampleSize = CalculateInSampleSize(options, imageSizer.Width, imageSizer.Height);

            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeResource(res, resId, options);
        }

        public Bitmap DecodeSampledBitmapFromUri(ContentResolver contentResolver, Uri uri, ISetImageSize imageSize, Bitmap fallback)
        {
            Stream input;
            try
            {
                input = contentResolver.OpenInputStream(uri);
            }
            catch (Exception)
            {
                return fallback;
            }

            var options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeStream(input, null, options);
            input.Close();
            options.InSampleSize = CalculateInSampleSize(options, imageSize.Width, imageSize.Height);
            options.InMutable = true;
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