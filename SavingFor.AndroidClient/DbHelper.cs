using System.IO;
using System.Threading;
using Android.App;

namespace SavingFor.AndroidClient
{
    internal sealed class DbHelper
    {
        private static DbHelper dbHelper;

        private DbHelper()
        {
            DbPath = CreateDatabasePath;

        }

        public static DbHelper GetSingleton()
        {
            if (dbHelper != null) return dbHelper;

            var temp = new DbHelper();
            Interlocked.CompareExchange(ref dbHelper, temp, null);
            return dbHelper;
        }

        public string DbPath { get; }

        private static string CreateDatabasePath
        {
            get
            {
                var path = Application.Context.ApplicationInfo.DataDir;
#if DEBUG
                return Path.Combine(path, "test_track_goal");
#else
                return Path.Combine(path,"track_goal");
#endif
            }
        }
    }
}