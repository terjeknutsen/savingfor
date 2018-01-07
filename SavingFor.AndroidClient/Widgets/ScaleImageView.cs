using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidClient.Widgets.Detectors;

namespace SavingFor.AndroidClient.Widgets
{
    [Register("com.track.goal.widgets.ScaleImageView")]
    public sealed class ScaleImageView : ImageView, View.IOnTouchListener
    {
        private readonly Context mContext;

        private const float MaxScale = 2.0f;

        private Matrix matrix;
        private readonly float[] matrixValues = new float[9];
        private int width;
        private int height;
        private int intrinsicWidth;
        private int intrinsicHeight;
        private float scale;
        private float minScale;
        private float previousDistance;
        private int previousMoveX;
        private int previousMoveY;

        private bool isScaling;
        private GestureDetector gestureDetector;
        private bool disposed = false;

        public ScaleImageView(Context context) : base(context)
        {
            mContext = context;
            Initialize();
        }
        public ScaleImageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            mContext = context;
            Initialize();
        }

        public ScaleImageView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            mContext = context;
            Initialize();
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            base.SetImageBitmap(bm);
            Initialize();
        }

        public override void SetImageResource(int resId)
        {
            base.SetImageResource(resId);
            Initialize();
        }

        private void Initialize()
        {
            SetScaleType(ScaleType.Matrix);
            matrix = new Matrix();

            if (Drawable != null)
            {
                intrinsicWidth = Drawable.IntrinsicWidth;
                intrinsicHeight = Drawable.IntrinsicHeight;
                SetOnTouchListener(this);
            }

            gestureDetector = new GestureDetector(mContext, new ScaleImageViewGestureDetector(this));
        }

        protected override bool SetFrame(int l, int t, int r, int b)
        {
            width = r - l;
            height = b - t;

            matrix.Reset();
            var rNorm = r - l;
            scale = (float)rNorm / intrinsicWidth;

            var paddingHeight = 0;
            var paddingWidth = 0;
            if (scale * intrinsicHeight > height)
            {
                scale = (float)height / intrinsicHeight;
                matrix.PostScale(scale, scale);
                paddingWidth = (r - width) / 2;
            }
            else
            {
                matrix.PostScale(scale, scale);
                paddingHeight = (b - height) / 2;
            }

            matrix.PostTranslate(paddingWidth, paddingHeight);
            ImageMatrix = matrix;
            minScale = scale;
            ZoomTo(scale, width / 2, height / 2);
            Cutting();
            return base.SetFrame(l, t, r, b);
        }

        private float GetValue(Matrix fromMatrix, int whichValue)
        {
            fromMatrix.GetValues(matrixValues);
            return matrixValues[whichValue];
        }



        public float Scale => GetValue(matrix, Matrix.MscaleX);

        public float TranslateX => GetValue(matrix, Matrix.MtransX);

        public float TranslateY => GetValue(matrix, Matrix.MtransY);
        public int StartX { get; set; }
        public int StartY { get; set; }

        public void MaxZoomTo(int x, int y)
        {
            if (Math.Abs(minScale - Scale) > float.Epsilon && (Scale - minScale) > 0.1f)
            {
                var tmpScale = minScale / Scale;
                ZoomTo(tmpScale, x, y);
            }
            else
            {
                var tmpScale = MaxScale / Scale;
                ZoomTo(tmpScale, x, y);
            }
        }

        public void ZoomTo(float scaleToUse, int x, int y)
        {
            if (Scale * scaleToUse < minScale)
            {
                scaleToUse = minScale / Scale;
            }
            else
            {
                if (scaleToUse >= 1 && Scale * scaleToUse > MaxScale)
                {
                    scaleToUse = MaxScale / Scale;
                }
            }
            matrix.PostScale(scaleToUse, scaleToUse);
            //move to center
            matrix.PostTranslate(-(width * scaleToUse - width) / 2, -(height * scaleToUse - height) / 2);

            //move x and y distance
            matrix.PostTranslate(-(x - (width / 2)) * scaleToUse, 0);
            matrix.PostTranslate(0, -(y - (height / 2)) * scaleToUse);
            ImageMatrix = matrix;
        }

        public void Cutting()
        {
            var tmpWidth = (int)(intrinsicWidth * Scale);
            var tmpHeight = (int)(intrinsicHeight * Scale);
            if (TranslateX < -(tmpWidth - width))
            {
                matrix.PostTranslate(-(TranslateX + tmpWidth - width), 0);
            }

            if (TranslateX > 0)
            {
                matrix.PostTranslate(-TranslateX, 0);
            }

            if (TranslateY < -(tmpHeight - height))
            {
                matrix.PostTranslate(0, -(TranslateY + tmpHeight - height));
            }

            if (TranslateY > 0)
            {
                matrix.PostTranslate(0, -TranslateY);
            }

            if (tmpWidth < width)
            {
                matrix.PostTranslate((width - tmpWidth) / 2.0f, 0);
            }

            if (tmpHeight < height)
            {
                matrix.PostTranslate(0, (height - tmpHeight) / 2.0f);
            }

            ImageMatrix = matrix;
        }

        private static float Distance(float x0, float x1, float y0, float y1)
        {
            var x = x0 - x1;
            var y = y0 - y1;
            return (float) Math.Sqrt(x * x + y * y);
        }

        private float DispDistance()
        {
            return (float) Math.Sqrt(width * width + height * height);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (gestureDetector.OnTouchEvent(e))
            {
                previousMoveX = (int)e.GetX();
                previousMoveY = (int)e.GetY();
                return true;
            }

            var touchCount = e.PointerCount;
            switch (e.Action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.Pointer1Down:
                case MotionEventActions.Pointer2Down:
                    {
                        if (touchCount >= 2)
                        {
                            var distance = Distance(e.GetX(0), e.GetX(1), e.GetY(0), e.GetY(1));
                            previousDistance = distance;
                            isScaling = true;
                        }
                    }
                    break;

                case MotionEventActions.Move:
                    {
                        if (touchCount >= 2 && isScaling)
                        {
                            var distance = Distance(e.GetX(0), e.GetX(1), e.GetY(0), e.GetY(1));
                            var tmpScale = (distance - previousDistance) / DispDistance();
                            previousDistance = distance;
                            tmpScale += 1;
                            tmpScale = tmpScale * tmpScale;
                            ZoomTo(tmpScale, width / 2, height / 2);
                            Cutting();
                        }
                        else if (!isScaling)
                        {
                            var distanceX = previousMoveX - (int)e.GetX();
                            var distanceY = previousMoveY - (int)e.GetY();
                            previousMoveX = (int)e.GetX();
                            previousMoveY = (int)e.GetY();

                            matrix.PostTranslate(-distanceX, -distanceY);
                            Cutting();
                        }
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                case MotionEventActions.Pointer2Up:
                    {
                        if (touchCount <= 1)
                        {
                            isScaling = false;
                        }
                    }
                    break;
            }
            return true;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            return OnTouchEvent(e);
        }

        protected override void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if (disposing)
                {
                    if (mContext != null)
                        mContext.Dispose();
                    if (gestureDetector != null)
                        gestureDetector.Dispose();
                    if(matrix != null)
                    matrix.Dispose();
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}