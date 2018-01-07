using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Java.IO;
using Java.Lang;
using Uri = Android.Net.Uri;

namespace SavingFor.AndroidLib.Utils
{
    public class ImageSelector : IDisposable
    {
        private readonly List<Intent> cameraIntents = new List<Intent>();
        private readonly IList<ResolveInfo> listCam;
        private readonly Intent captureIntent;
        private readonly Uri outputFileUri;
        private readonly ICharSequence selectString;
        private readonly int requestCode;
        private readonly Activity activity;
        private bool disposed = false;
        public ImageSelector(PackageManager packageManager, ICharSequence selectString, int requestCode,
            Activity activity)
        {
            var root = new File(Android.OS.Environment.ExternalStorageDirectory + File.Separator + "com.track.goal" +
                                 File.Separator);
            root.Mkdirs();
            var uniqueImageFileName = "goal_" + Guid.NewGuid();
            var sdImageMainDirectory = new File(root, uniqueImageFileName);
            outputFileUri = Uri.FromFile(sdImageMainDirectory);
            this.selectString = selectString;
            this.requestCode = requestCode;
            this.activity = activity;
            captureIntent = new Intent(MediaStore.ActionImageCapture);
            listCam = packageManager.QueryIntentActivities(captureIntent, 0);
        }
        ~ImageSelector()
        {
            Dispose(false);
        }

        public void MakeSelection()
        {
            var intent = new Intent(Intent.ActionPick);
            intent.SetType("image/*");

            activity.StartActivityForResult(intent, requestCode);
        }

        private static IParcelable ConvertToParceable(Intent intent)
        {
            return intent;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                cameraIntents.Clear();
                listCam.Clear();
                captureIntent.Dispose();
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