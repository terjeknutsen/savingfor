using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidClient.Adapters.Apis;
using SavingFor.AndroidLib.Apis;

namespace SavingFor.AndroidClient.Adapters.CommandObjects
{
    internal struct GoalCommand
    {
        public View View { get; set; }
        public ContentResolver Resolver { get; set; }
        public Typeface Typeface { get; set; }
        public Bitmap PlaceHolder { get; set; }
        public Resources Resources { get; set; }
        public Action<Guid,ImageView,string> ItemClickListener { get; set; }
        public Action<int> ItemLongClickListener { get; set; }
        public IModeable Mode { get; set; }
        public Action<ItemCheckChangedEvent> ItemSelected { get; set; }
        public IImageService ImageService { get; set; }
    }
}