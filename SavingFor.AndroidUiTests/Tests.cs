using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Android;

namespace SavingFor.AndroidUiTests
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            // TODO: If the Android app being tested is included in the solution then open
            // the Unit Tests window, right click Test Apps, select Add App Project
            // and select the app projects that should be tested.
            app = ConfigureApp
                .Android
                // TODO: Update this path to point to your Android app and uncomment the
                // code if the app is not included in the solution.
                .ApkFile("../../../SavingFor.AndroidClient/bin/Debug/no.savingfor.goal.apk")
                .StartApp();
        }

        [Test]
        public void AppLaunches()
        {
            app.SetOrientationLandscape();
            AddItem();
            DeleteItem();
        }
       
        public void AddItem()
        {
            app.Tap(c => c.Id("fab_button"));
            app.EnterText(c => c.Id("edit_text_name"), "test");
            app.PressEnter();
            app.EnterText(c => c.Marked("edit_text_amount"), "23.30");
            app.Tap(c => c.Id("fab_button_goal_edit"));
        }
     
        public void DeleteItem()
        {
            app.TouchAndHold(c => c.Marked("test"));
            app.Tap(c => c.Id("item_delete"));
            app.Tap(c => c.Id("button1"));
        }
    }
}

