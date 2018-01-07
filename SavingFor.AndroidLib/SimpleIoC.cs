using Android.Content;
using Android.Util;
using SavingFor.AndroidLib.Apis;
using SavingFor.AndroidLib.Bitmaps;
using SavingFor.AndroidLib.Bitmaps.Decorators;
using SavingFor.AndroidLib.Services;

namespace SavingFor.AndroidLib
{
    public static class SimpleIoC
    {
        private static IImageService imageService;
        private static IBitmapRepository bitmapRepository;
        private static IBlurImage imageBlurer;

        public static IImageService GetImageService(Context context, DisplayMetrics displayMetrics)
        {
            return imageService ?? (imageService = new ImageService(GetBitmapResizer(), GetBitmapRepository(), GetImageBlurer(context, displayMetrics)));
        }

        private static ILoadBitmap GetBitmapLoader()
        {
            return new BitmapLoader();
        }

        private static IResizeBitmap GetBitmapResizer()
        {
            return new BitmapResizer();
        }

        public static IBitmapRepository GetBitmapRepository()
        {
            return bitmapRepository ?? (bitmapRepository = new LruCacheBitmapRepository(new BitmapRepository()));
        }

        public static IBlurImage GetImageBlurer(Context context, DisplayMetrics displayMetrics)
        {
            return imageBlurer ?? (imageBlurer = new BitmapBlurer(context, displayMetrics));
        }
    }
}