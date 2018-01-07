using System.Threading.Tasks;
using Android.Graphics;
using Android.Util;
using SavingFor.AndroidLib.Apis;
using System;

namespace SavingFor.AndroidLib.Bitmaps.Decorators
{
    class LruCacheBitmapRepository : IBitmapRepository,IDisposable
    {
        private readonly IBitmapRepository decorated;
        private const int MaxSize = 1024 * 16;
        private readonly LruCache cache = new LruCache(MaxSize);
        private bool disposed;

        public LruCacheBitmapRepository(IBitmapRepository decorated)
        {
            this.decorated = decorated;
        }

        ~LruCacheBitmapRepository()
        {
            Dispose(false);
        }
        


        public string Save(Bitmap bitmap, string name)
        {
            return decorated.Save(bitmap, name);
        }

        public void Delete(string image)
        {
            cache.Remove(image);
            decorated.Delete(image);
        }

        public Bitmap Get(string image)
        {
            if (string.IsNullOrEmpty(image)) return decorated.Get(image);
            var bitmap = cache.Get(image) as Bitmap;
            if (bitmap != null) return bitmap;
            
            bitmap = decorated.Get(image);
            if(bitmap!=null)
            cache.Put(image, bitmap);

            return bitmap;
        }

        public async Task<Bitmap> GetAsync(string image)
        {
            if (string.IsNullOrEmpty(image)) return await decorated.GetAsync(image);
            var bitmap = cache.Get(image) as Bitmap;
            if (bitmap != null) return await Task.Run(()=> bitmap);

            bitmap = await decorated.GetAsync(image);
            if (bitmap != null)
                cache.Put(image, bitmap);
            return bitmap;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    cache.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}