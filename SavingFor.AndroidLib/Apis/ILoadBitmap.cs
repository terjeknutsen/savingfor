using System;
using Android.Content;
using Android.Graphics;
using Android.Util;

namespace SavingFor.AndroidLib.Apis
{
    public interface ILoadBitmap
    {
        Bitmap Load(Android.Net.Uri uri, ContentResolver contentResolver, DisplayMetrics displayMetrics);
    }
}