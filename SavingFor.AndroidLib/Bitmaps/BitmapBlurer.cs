using System;
using Android.Content;
using Android.Graphics;
using Android.Renderscripts;
using Android.Util;
using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidLib.Bitmaps
{
    class BitmapBlurer : IBlurImage
    {
        private readonly Context context;
        private readonly DisplayMetrics metrics;

        public BitmapBlurer(Context context, DisplayMetrics metrics)
        {
            this.context = context;
            this.metrics = metrics;
        }

        public Bitmap Blur(string key, Bitmap bitmap,Color color)
        {
            if (bitmap == null) return null;
            var red = Color.GetRedComponent(color);
            var blue = Color.GetBlueComponent(color);
            var green = Color.GetGreenComponent(color);
            var blurColor = new Color(red,green,blue,66);
           
            var px = 70f * ((float)metrics.DensityDpi / 160f);
            var pxs = Math.Round(px);

            var ratio = pxs / bitmap.Height;

            var height = (ratio * bitmap.Height);
            var y = bitmap.Height - height;


            var portionToBlur = Bitmap.CreateBitmap(bitmap, 0, (int)y, bitmap.Width,
                (int)height);
            var blurredBitmap = portionToBlur.Copy(Bitmap.Config.Argb8888, true);
            var renderScript = RenderScript.Create(context);
            var theIntrinsic = ScriptIntrinsicBlur.Create(renderScript, Element.U8_4(renderScript));
            var tmpIn = Allocation.CreateFromBitmap(renderScript, portionToBlur);
            var tmpOut = Allocation.CreateFromBitmap(renderScript, blurredBitmap);
            theIntrinsic.SetRadius(25f);
            theIntrinsic.SetInput(tmpIn);
            theIntrinsic.ForEach(tmpOut);
            tmpOut.CopyTo(blurredBitmap);
            new Canvas(blurredBitmap).DrawColor(blurColor);
            var newBitmap = bitmap.Copy(Bitmap.Config.Argb8888, true);
            var canvas = new Canvas(newBitmap);
            canvas.DrawBitmap(blurredBitmap, 0, (int)y, new Paint());
            return newBitmap;
        }
    }
}