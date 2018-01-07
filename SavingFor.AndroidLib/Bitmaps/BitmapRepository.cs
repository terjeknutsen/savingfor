using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Util;
using SavingFor.AndroidLib.Apis;
using Environment = System.Environment;
using Path = System.IO.Path;
using File = Java.IO.File;

namespace SavingFor.AndroidLib.Bitmaps
{
    public sealed class BitmapRepository : IBitmapRepository
    {
        readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public string Save(Bitmap bitmap, string name)
        {
            var filename = Path.Combine(path, name);
            var stream = new FileStream(filename, FileMode.OpenOrCreate);
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();

            return filename;
        }

        public void Delete(string image)
        {
            TryDelete(image);
            TryDelete($"{image}_medium");
            TryDelete($"{image}_small");
        }

        private void TryDelete(string image)
        {
            var fileName = GetFileNameFor(image);
            var file = new File(fileName);
            if (file.Exists())
                file.Delete();
        }

        private string GetFileNameFor(string image)
        {
            return Path.Combine(path, image);
        }

        public Bitmap Get(string image)
        {
            if (string.IsNullOrEmpty(image)) return null;
            var fileName = Path.Combine(path, image);
            return BitmapFactory.DecodeFile(fileName);
        }

        public async Task<Bitmap> GetAsync(string image)
        {
            if (string.IsNullOrEmpty(image)) return null;
            var fileName = Path.Combine(path, image);
            return await BitmapFactory.DecodeFileAsync(fileName);
        }
    }
}