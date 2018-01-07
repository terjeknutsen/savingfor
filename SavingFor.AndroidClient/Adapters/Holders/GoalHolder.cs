using System;
using System.Globalization;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidClient.Adapters.CommandObjects;
using SavingFor.AndroidLib.ImageHandling;
using SavingFor.AndroidLib.Palettes;
using SavingFor.AndroidLib.Utils;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Adapters.Holders
{
    class GoalHolder : RecyclerView.ViewHolder
    {
        private readonly GoalCommand command;
        private GoalItemAdapter.GoalItemState state;
        private TextView progressText;
        private ImageView imageView;
        private TextView itemAmount;
        private TextView title;
        private Color currentPalette;

        public GoalHolder(GoalCommand command) : base(command.View)
        {
            this.command = command;
            command.View.Click += delegate { OnItemClick(); };
            command.View.LongClick += delegate { OnItemLongClick(); };
            command.Mode.ModeChanged += OnModeChanged;
            command.Mode.Refresh += OnRefresh;
            command.View.FindViewById<CheckBox>(Resource.Id.check_box_goal_item).CheckedChange += (s, e) => OnItemSelected(e);
            BindView();
        }

        private void BindView()
        {
            progressText = ItemView.FindViewById<TextView>(Resource.Id.text_view_goal_item_progress);
            progressText.Typeface = command.Typeface;
            imageView = ItemView.FindViewById<ImageView>(Resource.Id.image_view_goal_item);
            itemAmount = ItemView.FindViewById<TextView>(Resource.Id.text_view_goal_item_amount);
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            SetProgress(state.GoalAmountRequired);
        }

        private void OnItemSelected(CompoundButton.CheckedChangeEventArgs e)
        {
            state.IsChecked = e.IsChecked;
            command.ItemSelected(new ItemCheckChangedEvent { IsChecked = e.IsChecked, Position = AdapterPosition });

            if (e.IsChecked)
            {
                SetElevated();
            }
            else
            {
                RemoveElevation();
            }
        }

        private void OnModeChanged(object sender, bool isEditmode)
        {
            state.IsEditMode = isEditmode;
            var checkbox = command.View.FindViewById<CheckBox>(Resource.Id.check_box_goal_item);
            if (!isEditmode)
            {
                checkbox.Checked = false;
                state.IsChecked = false;
                checkbox.Visibility = ViewStates.Gone;
                RemoveElevation();
            }
            else
            {
                checkbox.Visibility = ViewStates.Visible;
            }
        }

        private void OnItemClick()
        {
            if (state.IsEditMode)
            {
                ToogleCheckbox(state.IsChecked);
                return;
            }
            ViewCompat.SetTransitionName(imageView,Resources.GetString(Resource.String.transition_goal_image));
            command.ItemClickListener(state.Id,imageView, HexConverter(currentPalette));
        }
        private static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private void OnItemLongClick()
        {
            var checkbox = command.View.FindViewById<CheckBox>(Resource.Id.check_box_goal_item);
            if (checkbox.Visibility == ViewStates.Visible)
            {
                checkbox.Checked = !checkbox.Checked;
                state.IsChecked = checkbox.Checked;
            }
            else
            {
                command.ItemLongClickListener(AdapterPosition);
                SetChecked();
            }
        }

        private void SetChecked()
        {
            command.View.FindViewById<CheckBox>(Resource.Id.check_box_goal_item).Checked = true;
            state.IsChecked = true;
        }

        public Resources Resources { get; set; }

        public void SetGoal(GoalItemAdapter.GoalItemState goalState)
        {
            state = goalState;
            state.Position = AdapterPosition;
            SetImageWithPalette(goalState.Image);
            SetTitle(goalState.Name);
            SetDate(goalState.End);
            SetAmount(goalState.Amount);
            SetProgress(goalState.GoalAmountRequired);
            ToggleElevation(goalState.IsChecked);
            SetCheckbox(goalState.IsChecked);
            ToogleEditmode(goalState.IsEditMode);
        }


        private void SetCheckbox(bool isChecked)
        {
            ItemView.FindViewById<CheckBox>(Resource.Id.check_box_goal_item).Checked = isChecked;
        }

        private void ToogleEditmode(bool isEditMode)
        {
            command.View.FindViewById<CheckBox>(Resource.Id.check_box_goal_item).Visibility = isEditMode ? ViewStates.Visible : ViewStates.Gone;
        }

        private void ToogleCheckbox(bool isChecked)
        {
            state.IsChecked = !isChecked;
            ItemView.FindViewById<CheckBox>(Resource.Id.check_box_goal_item).Checked = !isChecked;
        }

        private void ToggleElevation(bool isChecked)
        {
            if (isChecked) SetElevated();
            else RemoveElevation();
        }

        private void SetElevated()
        {
            ItemView.FindViewById<CardView>(Resource.Id.card_view_goal_list_item).CardElevation = 12;
        }

        private void RemoveElevation()
        {
            ItemView.FindViewById<CardView>(Resource.Id.card_view_goal_list_item).CardElevation = 4;
        }

        private void SetImageWithPalette(string image)
        {
            LoadBitmap(image, imageView);
            if (string.IsNullOrEmpty(image))
            {
                SetDefaultPalette();
            }
        }

        private void SetDefaultPalette()
        {
            var bitmap = BitmapDecoder.DecodeSampledBitmapFromResource(command.Resources,
                    Resource.Drawable.ic_broken_image_white_48dp, new TileImageSize(100));
            var palette = new PalettUtil();
            var tileTextPalette = new TileTextPaletteDecorator(palette, bitmap);
            currentPalette = tileTextPalette.Color;
            tileTextPalette.SetPalette(ItemView.FindViewById<RelativeLayout>(Resource.Id.relative_layout_goal_item_info_tile));
            tileTextPalette.SetPalette(ItemView.FindViewById<TextView>(Resource.Id.text_view_goal_item_date));
        }

        public void LoadBitmap(string image, ImageView imageView)
        {
            if (!CancelPotentialWork(image, imageView)) return;

            var task = CreateBitmapWorkerTask(imageView);
           
            var asyncDrawable = new AsyncDrawable(command.Resources, command.PlaceHolder, task);
            imageView.SetImageDrawable(asyncDrawable);
            task.Execute(image);

        }

        private BitmapWorkerTask CreateBitmapWorkerTask(ImageView imageView)
        {
            return new BitmapWorkerTask(imageView, command.ImageService, color => currentPalette = color)
            {
                ContentResolver = command.Resolver,
                TextRootView = ItemView.FindViewById<RelativeLayout>(Resource.Id.relative_layout_goal_item_info_tile),
                TextDateView = ItemView.FindViewById<TextView>(Resource.Id.text_view_goal_item_date),
            };
        }

        private static bool CancelPotentialWork(string image, ImageView imageView)
        {
            var bitmapWorkerTask = GetBitmapWorkerTask(imageView) as BitmapWorkerTask;
            if (bitmapWorkerTask == null) return true;

            var path = bitmapWorkerTask.CurrentImage;
            if (path == null || path != image)
            {
                try
                {
                    bitmapWorkerTask.Cancel(true);
                }
                catch (ObjectDisposedException e)
                {
                    Log.Debug("Bitmap", $"bitmapWorkerTask disposed: {e.StackTrace}");
                }
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

        private void SetTitle(string name)
        {
            title = ItemView.FindViewById<TextView>(Resource.Id.text_view_goal_item_name);
            title.Typeface = command.Typeface;
            title.Text = name;
        }

        private void SetDate(DateTime end)
        {
            ItemView.FindViewById<TextView>(Resource.Id.text_view_goal_item_date).Text = end.ToString("dd. MMM",CultureInfo.CurrentCulture);
            if (end.Year == DateTime.Now.Year) return;
            ItemView.FindViewById<TextView>(Resource.Id.text_view_goal_item_year).Text = end.ToString("yyyy");
        }

        private void SetProgress(GoalAmountRequired goalAmountRequired)
        {
            progressText.Text = goalAmountRequired.RequiredAmount().ToString("C");
        }

        private void SetAmount(decimal amount)
        {
           itemAmount.Text = amount.ToString("C");
        }
    }
}