using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidLib.Utils
{
    public struct TileImageSize : ISetImageSize
    {
        public TileImageSize(int size)
        {
            Width = (int)(size * 1.618f);
            Height = size;
        }

        public int Width { get; }

        public int Height { get; }
    }
}