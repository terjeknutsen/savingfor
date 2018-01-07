using System.Threading.Tasks;
using Android.Graphics;
using Uri = Android.Net.Uri;

namespace SavingFor.AndroidLib.Apis
{
    public interface IImageService
    {
        Task<string> Add(Uri uri, Bitmap bitmap);
        void Remove(string image);
        Bitmap GetBitmap(string image);
        Bitmap Blur(string key, Bitmap bitmap,Color color);
        Task<Bitmap> GetBitmapAsync(string image);
    }
}