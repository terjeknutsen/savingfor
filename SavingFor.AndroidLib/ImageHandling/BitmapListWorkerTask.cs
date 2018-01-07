using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using SavingFor.AndroidLib.Apis;
using Object = Java.Lang.Object;

namespace SavingFor.AndroidLib.Utils
{
    public sealed class BitmapListWorkerTask : AsyncTask
    {
        private readonly IImageService imageService;
        private readonly WeakReference weakReference;

        public BitmapListWorkerTask(ImageView imageView, IImageService imageService)
        {
            this.imageService = imageService;
            weakReference = new WeakReference(imageView);
        }

        protected override Object DoInBackground(params Object[] @params)
        {
            var path = @params[0].ToString();
            if (string.IsNullOrEmpty(path)) return null;

            CurrentImage = path;

            try
            {
                var bitmap = imageService.GetBitmap(path);

                return GetCircleBitmap(bitmap);

            }
            catch (Exception)
            {

                return null;
            }
        }

        private static Bitmap MakeCircle(Bitmap bitmap)
        {
            float width = bitmap.Width;
            float height = bitmap.Width;
            var circleBitmap = Bitmap.CreateBitmap((int)width + 50, (int)height + 50, Bitmap.Config.Argb8888);
            var shader = new BitmapShader(bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

            var paint = new Paint();
            paint.SetShader(shader);
            paint.SetStyle(Paint.Style.Fill);
            paint.AntiAlias = true;
            var canvas = new Canvas(circleBitmap);

            canvas.DrawCircle(width / 2, height / 2, width / 2, paint);
            return circleBitmap;

        }
        private static Bitmap GetCircleBitmap(Bitmap bitmap)
        {
            var output = Bitmap.CreateBitmap(bitmap.Width,
             bitmap.Height, Bitmap.Config.Argb8888);
            var canvas = new Canvas(output);

            var color = Color.Red;
            var paint = new Paint();
            var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            var rectF = new RectF(rect);

            paint.AntiAlias = true;
            canvas.DrawARGB(0, 0, 0, 0);
            paint.Color = color;
            canvas.DrawOval(rectF, paint);

            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(bitmap, rect, rect, paint);

            

            return output;
        }

        protected override void OnPostExecute(Object result)
        {
            base.OnPostExecute(result);

            if (IsCancelled)
            {
                Log.Debug("GOAL", "OnPostExecute - Task cancelled");
            }
            else
            {
                using (var bitmap = result as Bitmap)
                {
                    if (weakReference == null || bitmap == null) return;
                    var view = weakReference.Target as ImageView;
                    view?.SetAdjustViewBounds(true);
                    view?.SetImageBitmap(bitmap);
                }
            }
        }

        public ISetImageSize ImageSizer { get; set; }

        public ContentResolver ContentResolver { get; set; }
        public string CurrentImage { get; private set; }
    }
}