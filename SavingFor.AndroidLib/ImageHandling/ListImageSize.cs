using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidLib.Utils
{
    public struct ListImageSize : ISetImageSize
    {
        public ListImageSize(int size)
        {
            Width = size;
            Height = size;
        }
        public int Width { get; }
        public int Height { get; }
    }
}