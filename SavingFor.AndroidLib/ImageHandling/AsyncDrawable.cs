using System;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;

namespace SavingFor.AndroidLib.Utils
{
    public class AsyncDrawable : BitmapDrawable
    {
        private readonly WeakReference bitmapWorkerTaskReference;

        public AsyncDrawable(Resources res, Bitmap bitmap, AsyncTask bitmapWorkerTask) : base(res, bitmap)
        {
            bitmapWorkerTaskReference = new WeakReference(bitmapWorkerTask);
        }

        public AsyncTask BitmapWorkerTask => bitmapWorkerTaskReference.Target as AsyncTask;
    }
}