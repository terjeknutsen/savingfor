using Android.App;
using Android.Content;
using Android.Graphics;

namespace SavingFor.AndroidClient.Settings
{
    static class Preferences
    {
        private static readonly ISharedPreferences SharedPreferences;
        private const string IndexToken = "com.savingfor.main_image_index";
        private const string HeroColor = "com.savingfor.hero_color";

        static Preferences()
        {
           SharedPreferences = Application.Context.GetSharedPreferences("com.savingfor", FileCreationMode.Private);
        }

        public static string HeroImageGoalId
        {
            get
            {
                return SharedPreferences.GetString(IndexToken,"none");
            }
            set
            {
                var editor = SharedPreferences.Edit();
                editor.PutString(IndexToken, value);
                editor.Commit();
            }
        }

        public static Color CurrentColor
        {
            get
            {
                var colorString = SharedPreferences.GetString(HeroColor, "#FF353535");
                var color = Color.ParseColor(colorString);
                return color;
            }
            set
            {
                var colorString = HexConverter(value);
                var editor = SharedPreferences.Edit();
                editor.PutString(HeroColor, colorString);
                editor.Commit();
            }
        }

        public static Color CurrentColorDark
        {
            get
            {
                var red = CurrentColor.R > 19 ? CurrentColor.R - 20 : 0;
                var blue = CurrentColor.B > 19 ? CurrentColor.B - 20 : 0;
                var green = CurrentColor.G > 19 ? CurrentColor.G -20 : 0;
                return new Color(red,green,blue);

            }
        }

        public static bool IsDefaultColor(Color color)
        {
            return "#FF000000" == HexConverter(color);
        }

        private static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

    }
}