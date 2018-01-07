using System;
using System.Threading.Tasks;
using Android.Graphics;
using SavingFor.AndroidLib.Apis;
using Uri = Android.Net.Uri;
namespace SavingFor.AndroidLib.Services
{
    public sealed class ImageService : IImageService
    {
        private readonly IResizeBitmap bitmapResizer;
        private readonly IBitmapRepository bitmapRepository;
        private readonly IBlurImage imageBlurer;
        private const int SmallWidth = 120;
        private const int SmallHeight = 120;
        private const int MediumWidth = 350;
        private const int MediumHeight = 216;


        internal ImageService(IResizeBitmap bitmapResizer, IBitmapRepository bitmapRepository, IBlurImage imageBlurer)
        {
            this.bitmapResizer = bitmapResizer;
            this.bitmapRepository = bitmapRepository;
            this.imageBlurer = imageBlurer;
        }

        public async Task<string> Add(Uri uri, Bitmap bitmap)
        {
            if (bitmap == null) return string.Empty;
            var identifier = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() =>
            {
                Add(bitmap, $"{identifier}_small", SmallWidth, SmallHeight, true);
                Add(bitmap, $"{identifier}_medium", MediumWidth, MediumHeight, false);
                bitmapRepository.Save(bitmap, identifier);
            });

            bitmap.Recycle();
            return identifier;
        }

        private void Add(Bitmap bitmap, string name, int width, int height, bool rectangular)
        {
            var resizedBitmap = bitmapResizer.Resize(bitmap, width, height, rectangular);
            bitmapRepository.Save(resizedBitmap, name);
        }

        public void Remove(string image)
        {
            if (string.IsNullOrEmpty(image)) return;

            bitmapRepository.Delete(image);
        }

        public Bitmap GetBitmap(string image)
        {
            return bitmapRepository.Get(image);
        }

        public Bitmap Blur(string key, Bitmap bitmap, Color color)
        {
            return imageBlurer.Blur(key, bitmap,color);
        }

        public Task<Bitmap> GetBitmapAsync(string image)
        {
            return bitmapRepository.GetAsync(image);
        }
    }
}