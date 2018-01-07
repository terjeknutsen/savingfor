using System;
using Android.Content;
using Android.Graphics;
using Android.Views;

namespace SavingFor.AndroidClient.Widgets.Views
{
    public sealed class PaletteOne: View
    {
        public PaletteOne(Context context):base(context)
        {
            density = context.Resources.DisplayMetrics.Density;
            SetZ(5);
        }
        private Paint paint = new Paint();
        private float centerX = 1.0f;
        private float centerY = 1.0f;
        private float radius = 1.0f;
        private float density;
        public bool IsFinished => radius > 500;
        private Color Color => Resources.GetColor(Resource.Color.palette_1, default(Android.Content.Res.Resources.Theme));
        protected override void OnDraw(Canvas canvas)
        {
            centerX += 10;
            centerY += 10;
            radius += 10;
            paint.Reset();
            paint.AntiAlias = true;
            paint.SetStyle(Paint.Style.Fill);
            paint.Color = Color;
            canvas?.DrawCircle(centerX, centerY, radius * density, paint);
        }

    }
}