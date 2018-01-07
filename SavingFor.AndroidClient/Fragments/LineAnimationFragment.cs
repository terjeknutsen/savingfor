using System;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Fragment = Android.Support.V4.App.Fragment;
using System.Threading;
using System.Threading.Tasks;
using SavingFor.AndroidClient.Widgets.Views;

namespace SavingFor.AndroidClient.Fragments
{
    public class LineAnimationFragment : Fragment, ISurfaceHolderCallback
    {
        private ISurfaceHolder holder;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool disposed = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        protected override void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if (disposing)
                {
                    if(holder != null)
                    holder.Dispose();
                    if (cancellationTokenSource != null)
                        cancellationTokenSource.Dispose();
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_surface_view, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            holder = view.FindViewById<SurfaceView>(Resource.Id.surface_view).Holder;
            holder.AddCallback(this);
        }
        public override void OnPause()
        {
            base.OnPause();
            cancellationTokenSource.Cancel();
        }

        public void StartAnimation()
        {
            if (holder == null) return;
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            Task.Run(()=> 
            {
                var circularView = new PaletteOne(Activity);
                var paletteThree = new PaletteThree(Activity);
                var paletteTwo = new PaletteTwo(Activity);
                var paletteFour = new PaletteFour(Activity);
                var shadowFour = new ShadowFour(Activity);
                var paletteFive = new PaletteFive(Activity);
                while (!token.IsCancellationRequested)
                { 
                    //draw on the canvas
                    Canvas canvas = null;
                    
                    try
                    {
                        canvas = holder.LockCanvas();

                        circularView.Draw(canvas);

                        paletteThree.Draw(canvas);
                        paletteFive.Draw(canvas);
                        paletteTwo.Draw(canvas);
                        shadowFour.Draw(canvas);
                        paletteFour.Draw(canvas);
                       

                        if (paletteFour.IsFinished)
                        {
                            break;
                        }
                    }
                    catch(Exception)
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                    finally 
                    {
                        if(canvas != null)
                        {
                            holder.UnlockCanvasAndPost(canvas);
                        }
                    }
                }
            },token);
        }

        private void Animate()
        {

        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            this.holder = holder;
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            this.holder = null;
        }

        internal void StopAnimation()
        {
            cancellationTokenSource.Cancel();
        }
    }
}