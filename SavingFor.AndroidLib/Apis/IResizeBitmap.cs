using Android.Graphics;

namespace SavingFor.AndroidLib.Apis
{
    public interface IResizeBitmap
    {
        Bitmap Resize(Bitmap bitmap, int width, int height, bool rectangular);
    }
}