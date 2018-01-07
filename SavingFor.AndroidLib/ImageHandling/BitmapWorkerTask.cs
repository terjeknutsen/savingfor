using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidLib.Apis;
using SavingFor.AndroidLib.Palettes;
using Object = Java.Lang.Object;

namespace SavingFor.AndroidLib.ImageHandling
{
    public sealed class BitmapWorkerTask : AsyncTask
    {
        private readonly IImageService imageService;
        private readonly Action<Color> onPaletteSet;
        private readonly WeakReference imageViewReference;
        private TileTextPaletteDecorator tileTextPalette;

        public string CurrentImage;

        public BitmapWorkerTask(ImageView imageView, IImageService imageService, Action<Color> onPaletteSet )
        {
            this.imageService = imageService;
            this.onPaletteSet = onPaletteSet;
            imageViewReference = new WeakReference(imageView);
        }

        public ContentResolver ContentResolver { get; set; }
        public View TextRootView { get; set; }
        public View TextDateView { get; set; }

        protected override Object DoInBackground(params Object[] @params)
        {
            var path = @params[0].ToString();
            if (string.IsNullOrEmpty(path)) return null;

            CurrentImage = path;
            try
            {
                var bitmap = imageService.GetBitmap(path);
                TryCreatePalette(bitmap);
                return bitmap;
            }
            catch (Exception)
            {
                Log.Debug("GOAL", "exception while decoding image in background");
                return null;
            }
        }

        private void TryCreatePalette(Bitmap bitmap)
        {
            var paletteUtil = new PalettUtil();

            tileTextPalette = new TileTextPaletteDecorator(paletteUtil, bitmap);
            onPaletteSet(tileTextPalette.Color);
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
                    if (imageViewReference == null || bitmap == null) return;

                    var view = imageViewReference.Target as ImageView;
                    view?.SetImageBitmap(bitmap);

                    TrySetPalette();
                }
            }
        }

        private void TrySetPalette()
        {
            tileTextPalette?.SetPalette(TextRootView);
            tileTextPalette?.SetPalette(TextDateView);
        }
    }
}