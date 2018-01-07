using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SavingFor.AndroidClient.Extensions
{
    public static class Roboto
    {
        private static readonly IDictionary<string, Typeface> typefaces = new Dictionary<string, Typeface>();
        public static Typeface Black(AssetManager manager)
        {
            EnsureExists(manager, "Roboto-Black");

            return typefaces["Roboto-Black"];
        }

        public static Typeface Medium(AssetManager manager)
        {
            EnsureExists(manager, "Roboto-Medium");
            return typefaces["Roboto-Medium"];
        }

        public static Typeface Regular(AssetManager manager)
        {
            EnsureExists(manager, "Roboto-Regular");
            return typefaces["Roboto-Regular"];
        }

        public static Typeface CondensedRegular(AssetManager assets)
        {
            EnsureExists(assets, "RobotoCondensed-Regular");
            return typefaces["RobotoCondensed-Regular"];
        }

        public static Typeface LightItalic(AssetManager assets)
        {
            EnsureExists(assets, "Roboto-LightItalic");
            return typefaces["Roboto-LightItalic"];
        }

        public static Typeface Thin(AssetManager assets)
        {
            EnsureExists(assets, "Roboto-Thin");
            return typefaces["Roboto-Thin"];
        }

        private static void EnsureExists(AssetManager manager, string type)
        {
            if (!typefaces.ContainsKey(type))
            {
                typefaces.Add(type, Typeface.CreateFromAsset(manager, type + ".ttf"));
            }
        }
    }
}