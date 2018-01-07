using Android.Content;
using Android.Views;
using Android.Util;
using Android.Graphics;
using System;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public abstract class LineViewBase : View
    {
        public LineViewBase(Context context)
        :base(context){
            density = context.Resources.DisplayMetrics.Density;
            
        }
        public LineViewBase(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public LineViewBase(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        private Paint paint = new Paint();
        private float density;
        private int bottom = 10;

        protected abstract int GetLeft();
        protected abstract int GetRight();
        protected abstract int GetRotation();
        protected abstract int GetTop();

        protected abstract Color Color { get; }
        public bool IsFinished => bottom > 2000;
        protected override void OnDraw(Canvas canvas)
        {
            bottom += 10;
            paint.Reset();
            paint.AntiAlias = true;
            paint.SetStyle(Paint.Style.Fill);
            paint.Color = Color;
            var rect = new Rect(GetLeft(), GetTop(), (int)(GetRight() * density), bottom);
            canvas.Save();
            canvas.Rotate(GetRotation());
            canvas.DrawRect(rect, paint);
            canvas.Restore();
        }
    }
}