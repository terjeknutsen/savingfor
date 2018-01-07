using Android.Views;

namespace SavingFor.AndroidClient.Widgets.Detectors
{
    public class ScaleImageViewGestureDetector : GestureDetector.SimpleOnGestureListener
    {
        private readonly ScaleImageView scaleImageView;
        public ScaleImageViewGestureDetector(ScaleImageView imageView)
        {
            scaleImageView = imageView;
        }

        public override bool OnDown(MotionEvent e)
        {
            return true;
        }

        public override bool OnDoubleTap(MotionEvent e)
        {
            scaleImageView.MaxZoomTo((int)e.GetX(), (int)e.GetY());
            scaleImageView.Cutting();
            return true;
        }
    }
}