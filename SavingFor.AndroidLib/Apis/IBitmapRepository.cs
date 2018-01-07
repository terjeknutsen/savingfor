using System.Threading.Tasks;
using Android.Graphics;

namespace SavingFor.AndroidLib.Apis
{
    public interface IBitmapRepository
    {
        string Save(Bitmap bitmap, string name);
        void Delete(string image);
        Bitmap Get(string image);
        Task<Bitmap> GetAsync(string image);
    }
}