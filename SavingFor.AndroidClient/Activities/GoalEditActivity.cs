using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using SavingFor.AndroidClient.Extensions;
using SavingFor.AndroidClient.Factories;
using SavingFor.AndroidClient.Settings;
using SavingFor.AndroidLib;
using SavingFor.AndroidLib.Apis;
using SavingFor.AndroidLib.Utils;
using SavingFor.Domain.Model;
using UK.CO.Senab.Photoview;
using Uri = Android.Net.Uri;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.Security;

namespace SavingFor.AndroidClient.Activities
{
    [Activity(Label = "GoalEditActivity", Theme = "@style/material_theme", ParentActivity = typeof(MainActivity), ScreenOrientation = ScreenOrientation.Portrait)]
    public class GoalEditActivity : AppCompatActivity, ViewTreeObserver.IOnPreDrawListener
    {
        private Toolbar toolbar;
        private EditText editAmount;
        private FloatingActionButton fab;
        private EditText editName;
        private Button dateBtn;
        private Button imageBtn;
        private Uri selectedImageUri = Uri.Empty;
        private ImageSelector selector;
        public const string ColorPalette = "com.track.goal_palette";
        public const string GoalId = "com.track.goal.goal_id";
        private Goal goal;
        private DateTime selectedDate = DateTime.Now.AddDays(1);
        private CoordinatorLayout coordinatorLayout;
        private IImageService imageService;
        private ImageButton missingImageButton;
        private View imageWrapper;
        private PhotoView cropView;
        private View cropViewWrapper;
        private ProgressBar progressBar;
        private int currentImageGenerationId = -1;
        private ImageView existingImage;
        private RelativeLayout existingImageWrapper;
        public const string NewGoal = "com.track.goal.new_goal";
        const int ImageRequestCode = 233;
        private bool disposed = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_goal_edit_view);
            TrySetGoal();
            imageService = SimpleIoC.GetImageService(this, Resources.DisplayMetrics);
            SupportPostponeEnterTransition();
            BindView();
            RegisterClickEvents();

            selector = new ImageSelector(PackageManager, new Java.Lang.String("Velg"), ImageRequestCode, this);

            SetupToolbar();
            TrySetValues();
            StyleView();
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if(existingImageWrapper != null)
                    existingImageWrapper.Dispose();
                    if (existingImage != null)
                        existingImage.Dispose();
                    if (progressBar != null)
                        progressBar.Dispose();
                    if (cropViewWrapper != null)
                        cropViewWrapper.Dispose();
                    if (cropView != null)
                        cropView.Dispose();
                    if (imageWrapper != null)
                        imageWrapper.Dispose();
                    if (missingImageButton != null)
                        missingImageButton.Dispose();
                    if (coordinatorLayout != null)
                        coordinatorLayout.Dispose();
                    if (selector != null)
                        selector.Dispose();
                    if (imageBtn != null)
                        imageBtn.Dispose();
                    if (dateBtn != null)
                        dateBtn.Dispose();
                    if (editName != null)
                        editName.Dispose();
                    if (fab != null)
                        fab.Dispose();
                    if (editAmount != null)
                        editAmount.Dispose();
                    if (toolbar != null)
                        toolbar.Dispose();
                }

                disposed = true;
            }
            base.Dispose(disposing);
        }
        protected override void OnResume()
        {
            base.OnResume();

            if (cropView.VisibleRectangleBitmap != null)
            {
                currentImageGenerationId = cropView.VisibleRectangleBitmap.GenerationId;
            }
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            GC.Collect();
        }

        private void BindView()
        {
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_goal_edit);
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab_button_goal_edit);
            editAmount = FindViewById<EditText>(Resource.Id.edit_text_amount);

            editName = FindViewById<EditText>(Resource.Id.edit_text_name);
            editName.EditorAction += OnEditorAction;

            editAmount.EditorAction += OnAmountEdited;
            dateBtn = FindViewById<Button>(Resource.Id.button_goal_date);
            imageBtn = FindViewById<Button>(Resource.Id.button_goal_image);
            coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.coordinator_layout_edit_goal);
            missingImageButton = FindViewById<ImageButton>(Resource.Id.image_button_missing_image);
            imageWrapper = FindViewById<View>(Resource.Id.view_image_wrapper);
            cropView = FindViewById<PhotoView>(Resource.Id.crop_view);
            cropViewWrapper = FindViewById<View>(Resource.Id.view_crop_image_wrapper);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar_save);
            existingImage = FindViewById<ImageView>(Resource.Id.image_view_existing);
            existingImageWrapper = FindViewById<RelativeLayout>(Resource.Id.view_existing_image_wrapper);
            existingImage.ViewTreeObserver.AddOnPreDrawListener(this);
        }

        private void RegisterClickEvents()
        {
            dateBtn.Click += OnPickDate;
            imageBtn.Click += OnSelectImage;
            missingImageButton.Click += OnSelectImage;
            fab.Click += OnSaveGoal;
        }

        private void SetupToolbar()
        {
            if (toolbar == null)
            {
                SupportActionBar.Title = "";
                return;
            }

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.Title = "";
        }
        [SecuritySafeCritical]
        private void TrySetValues()
        {
            if (goal == null)
            {
                return;
            }

            SetImageBitmapFromGoal(goal.Image);
            SetAmount(goal.Amount);
            SetName(goal.Name);
            SetDate(goal.End);
        }

        private void SetImageBitmapFromGoal(string image)
        {
            if (string.IsNullOrEmpty(image)) return;

            var bitmap = imageService.GetBitmap(image);
            RunOnUiThread(() =>
            {
                missingImageButton.Visibility = ViewStates.Gone;
                cropViewWrapper.Visibility = ViewStates.Gone;
                existingImageWrapper.Visibility = ViewStates.Visible;
                existingImage.SetImageBitmap(bitmap);
            });
        }

        private void SetCropView(Uri data)
        {
            imageWrapper.Visibility = ViewStates.Gone;
            existingImageWrapper.Visibility = ViewStates.Gone;
            cropViewWrapper.Visibility = ViewStates.Visible;
            selectedImageUri = data;

            var imageSize = new AppBarImageSize(Resources.DisplayMetrics);
            var fallback = BitmapDecoder.DecodeSampledBitmapFromResource(Resources,
                    Resource.Drawable.ic_broken_image_white_48dp, imageSize);

            if (!string.IsNullOrEmpty(selectedImageUri?.Path))
            {
                Bitmap bitmap;
                using (var decoder = new BitmapDecoder())
                {
                    bitmap = decoder.DecodeSampledBitmapFromUri(ContentResolver, selectedImageUri, imageSize, fallback);

                    var w = bitmap.Width;

                    var scaledHeight = bitmap.Height * imageSize.Width / w;

                    if (scaledHeight < imageSize.Height)
                    {
                        var h = bitmap.Height;
                        var scaledWidth = bitmap.Width * imageSize.Height / h;
                        bitmap = Bitmap.CreateScaledBitmap(bitmap, scaledWidth, imageSize.Height, true);
                    }
                    else
                        bitmap = Bitmap.CreateScaledBitmap(bitmap, imageSize.Width, scaledHeight, true);
                }

                cropView.SetImageBitmap(bitmap);
            }

        }

        private void SetAmount(decimal amount)
        {
            editAmount.Text = amount.ToString("G");
        }

        private void SetName(string name)
        {
            editName.Text = name;
        }

        private void SetDate(DateTime end)
        {
            selectedDate = end;
            SupportActionBar.Subtitle = selectedDate.ToShortDateString();
            FindViewById<TextView>(Resource.Id.text_goal_date).Text = selectedDate.ToShortDateString();
        }
        [SecuritySafeCritical]
        private void TrySetGoal()
        {
            if (Intent.GetBooleanExtra(NewGoal, false)) return;

            goal = RepositoryFactory.GetSingleton().GetRepository().Get(new Guid(Intent.GetStringExtra(GoalId)));
        }

        [SecuritySafeCritical]
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId != Android.Resource.Id.Home) return base.OnOptionsItemSelected(item);
            var intent = NavUtils.GetParentActivityIntent(this);
            if (goal != null)
            Preferences.HeroImageGoalId = goal.Id.ToString();

            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            NavUtils.NavigateUpTo(this, intent);
            
            OverridePendingTransition(Resource.Animation.abc_fade_in,Resource.Animation.abc_fade_out);
            return true;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok && requestCode == ImageRequestCode)

                SetCropView(data.Data);

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void OnEditorAction(object sender, TextView.EditorActionEventArgs args)
        {

            if (string.IsNullOrEmpty(editName.Text))
            {
                ShowValidationError(FindViewById<TextInputLayout>(Resource.Id.text_input_layout_name),
                    Resources.GetString(Resource.String.name_is_required));
            }
            else
            {
                FindViewById<TextInputLayout>(Resource.Id.text_input_layout_name).Error = string.Empty;
            }

            editAmount.RequestFocus();
        }

        private void OnAmountEdited(object sender, TextView.EditorActionEventArgs args)
        {
            if (string.IsNullOrEmpty(editAmount.Text))
            {
                ShowValidationError(FindViewById<TextInputLayout>(Resource.Id.text_input_layout_amount),
                    Resources.GetString(Resource.String.could_not_read_amount));
            }
            else
            {
                FindViewById<TextInputLayout>(Resource.Id.text_input_layout_amount).Error = string.Empty;
            }

            var im = GetSystemService(InputMethodService) as InputMethodManager;
            im?.HideSoftInputFromWindow(editName.WindowToken, 0);

        }


        private void StyleView()
        {
            dateBtn.Typeface = Roboto.Medium(Assets);
            imageBtn.Typeface = Roboto.Medium(Assets);
        }

        private async void OnSaveGoal(object sender, EventArgs e)
        {
            StartSpinner();

            var saved = await TrySave();
            StopSpinner();

            if (!saved) return;

            var intent = NavUtils.GetParentActivityIntent(this);
            intent.PutExtra(IntentExtra.GoalAdded, true);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            NavUtils.NavigateUpTo(this, intent);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
        }

        private async Task<bool> TrySave()
        {
            if (!Validate(out decimal amount, out string name)) return false;

            var date = selectedDate;

            var isImageSelected = selectedImageUri != Uri.Empty;
            var viewHasBeenAltered = new Func<bool>(
                () => currentImageGenerationId == cropView?.VisibleRectangleBitmap?.GenerationId).Invoke();

            var imageName =
                (isImageSelected || viewHasBeenAltered)
                    ? await TryUpdateImage()
                    : goal?.Image;

            var tmp = GoalFactory.Create(amount, date,
                imageName, name);

           

            if (goal != null)
            {
                var updatedGoal = GoalFactory.Copy(goal, tmp);
                if (Compare(updatedGoal, goal))
                    return true;

                RepositoryFactory.GetSingleton().GetRepository().Update(updatedGoal);
            }

            else
            {
                RepositoryFactory.GetSingleton().GetRepository().Add(tmp);
            }

            return true;
        }

        private bool Validate(out decimal amount, out string name)
        {
            var parsed = decimal.TryParse(editAmount.Text, out amount);
            name = editName.Text;
            if (!parsed)
            {
                ShowValidationError(FindViewById<TextInputLayout>(Resource.Id.text_input_layout_amount),
                    Resources.GetString(Resource.String.could_not_read_amount));
                return false;
            }

            
            if (string.IsNullOrEmpty(name))
            {
                ShowValidationError(FindViewById<TextInputLayout>(Resource.Id.text_input_layout_name),
                    Resources.GetString(Resource.String.name_is_required));
                return false;
            }

            if (!selectedDate.Equals(default(DateTime))) return true;

            RunOnUiThread(
                () =>
                {
                    Snackbar.Make(coordinatorLayout, Resources.GetString(Resource.String.could_not_read_date),
                        Snackbar.LengthIndefinite).SetAction("OK", v => { }).Show();
                });
            return false;
        }
        [SecuritySafeCritical]
        private static bool Compare(Goal a, Goal b)
        {
            if (a == null || b == null) return false;
            return
                a.Amount == b.Amount &&
                a.End == b.End &&
                a.Name == b.Name &&
                a.Image == b.Image;
        }

        private void StartSpinner()
        {
            progressBar.Visibility = ViewStates.Visible;
        }

        private void StopSpinner()
        {
            progressBar.Visibility = ViewStates.Gone;
        }

        private async Task<string> TryUpdateImage()
        {
            if (goal?.Image != null)
                imageService.Remove(goal.Image);
            return await imageService.Add(selectedImageUri, cropView.VisibleRectangleBitmap);
        }

        private static void ShowValidationError(TextInputLayout textInputLayout, string message)
        {
            textInputLayout.Error = message;
        }

        private void OnSelectImage(object sender, EventArgs e)
        {
            selector.MakeSelection();
            GC.Collect();
        }

        private void OnPickDate(object sender, EventArgs e)
        {
            var dialog = new DatePickerDialog(this, OnDateSelected, selectedDate.Year, selectedDate.Month - 1, selectedDate.Day);
            dialog.Show();
        }

        private void OnDateSelected(object sender, DatePickerDialog.DateSetEventArgs e)
        {

            RunOnUiThread(() =>
            {
                var date = new DateTime(e.Year, e.Month + 1, e.DayOfMonth);
                var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                SetDate(utcDate);
            });
        }

        public bool OnPreDraw()
        {
            existingImage.ViewTreeObserver.RemoveOnPreDrawListener(this);
            SupportStartPostponedEnterTransition();
            return true;
        }
    }
}