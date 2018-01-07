using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidLib.Apis;
using SavingFor.AndroidLib.Utils;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Adapters.Holders
{
    sealed class GoalFinishedHolder : RecyclerView.ViewHolder
    {
        private readonly Action<Guid> onClick;
        private readonly Action<Guid> onLongClick;
        private ImageView imageView;
        private TextView name;
        private TextView date;
        private TextView amount;
        private CheckBox checkBox;
        private readonly IImageService imageService;

        public GoalFinishedHolder(View itemView, Action<Guid> onClick, Action<Guid> onLongClick, IImageService imageService) : base(itemView)
        {
            this.onClick = onClick;
            this.onLongClick = onLongClick;
            BindView(itemView);
            this.imageService = imageService;
        }

        private void BindView(View view)
        {
            imageView = view.FindViewById<ImageView>(Resource.Id.image_view_finished);
            name = view.FindViewById<TextView>(Resource.Id.text_name_finished);
            date = view.FindViewById<TextView>(Resource.Id.text_date_finished);
            amount = view.FindViewById<TextView>(Resource.Id.text_amount_finished);
            checkBox = view.FindViewById<CheckBox>(Resource.Id.checkbox_finished);
            var root = view.FindViewById<RelativeLayout>(Resource.Id.relative_root_finished);
            root.Click += delegate { onClick(Id); };
            root.LongClick += delegate { onLongClick(Id); };
            checkBox.Click += delegate { onClick(Id); };
        }

        public void SetItem(Goal goal, bool editMode, bool isChecked)
        {
            Id = goal.Id;
            SetImage(goal.ImageSmall);
            SetName(goal.Name);
            SetDate(goal.End);
            SetAmount(goal.Amount);
            ToggleCheckBox(editMode, isChecked);
        }

        private void SetImage(string imagePath)
        {
            LoadBitmap(imagePath);
        }

        public void LoadBitmap(string imagePath)
        {
            if (!CancelPotentialWork(imagePath, imageView)) return;

            var task = CreateBitmapWorkerTask();
            var asyncDrawable = new AsyncDrawable(Resources, Placeholder, task);
            imageView.SetImageDrawable(asyncDrawable);
            task.Execute(imagePath);
        }

        public Guid Id { get; private set; }

        public Bitmap Placeholder { get; set; }

        public Resources Resources { get; set; }

        public ContentResolver Resolver { get; set; }

        private BitmapListWorkerTask CreateBitmapWorkerTask()
        {

            return new BitmapListWorkerTask(imageView, imageService)
            {
                ContentResolver = Resolver,
            };

        }

        private static bool CancelPotentialWork(string imagePath, ImageView imageView)
        {
            var task = GetBitmapWorkerTask(imageView) as BitmapListWorkerTask;
            if (task == null) return true;

            var bitmapPath = task.CurrentImage;
            if (bitmapPath == null || bitmapPath != imagePath)
            {
                task.Cancel(true);
            }
            else
            {
                return false;
            }

            return true;
        }

        private static AsyncTask GetBitmapWorkerTask(ImageView imageView)
        {
            var drawable = imageView?.Drawable as AsyncDrawable;

            return drawable?.BitmapWorkerTask;
        }

        private void SetName(string s)
        {
            name.Text = s;
        }

        private void SetDate(DateTime end)
        {
            date.Text = end.AddDays(1).ToShortDateString();
        }

        private void SetAmount(decimal a)
        {
            amount.Text = a.ToString("C");
        }

        private void ToggleCheckBox(bool editMode, bool isChecked)
        {
            if (editMode)
            {
                checkBox.Visibility = ViewStates.Visible;
                imageView.Visibility = ViewStates.Gone;
                checkBox.Checked = isChecked;
            }
            else
            {
                checkBox.Visibility = ViewStates.Gone;
                imageView.Visibility = ViewStates.Visible;
                checkBox.Checked = false;
            }
        }
    }
}