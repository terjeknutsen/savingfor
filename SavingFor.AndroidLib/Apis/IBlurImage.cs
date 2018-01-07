using Android.Graphics;

namespace SavingFor.AndroidLib.Apis
{
    public interface IBlurImage
    {
        Bitmap Blur(string key, Bitmap bitmap,Color color);
    }
}