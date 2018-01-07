using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SavingFor.LPI;
using SavingFor.Domain.Model;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;

namespace SavingFor.Data.SqlServer
{
    public sealed class GoalRepositoryAndroid : IRepository<Goal>
    {
        private readonly ISQLitePlatform sqlitePlatform = new SQLitePlatformAndroid();
        private readonly string path;
        readonly object myLock = new object();
        public GoalRepositoryAndroid(string path)
        {
            this.path = path;
            lock (myLock)
            {
                using (var connection = new SQLiteConnection(sqlitePlatform, path))
                {
                    connection.CreateTable<Goal>(CreateFlags.AutoIncPK);
                    connection.Commit();
                    connection.Close(); 
                }
            }
        }

        public void Add(Goal goal)
        {
            lock (myLock)
            {
                using (var connection = CreateConnection)
                {
                    connection.Insert(goal);
                    connection.Commit();
                    connection.Close();
                }
            }
        }

        public void Update(Goal goal)
        {
            lock (myLock)
            {
                using (var connection = CreateConnection)
                {
                    connection.Update(goal);
                    connection.Commit();
                    connection.Close();
                }
            }
        }

        public Goal Get(int goalId)
        {
            lock (myLock)
            {
                using (var connection = CreateConnection)
                {
                    var item = connection.Get<Goal>(goalId);
                    connection.Commit();
                    connection.Close();
                    return item;
                }
            }
        }

        private SQLiteConnection CreateConnection => new SQLiteConnection(sqlitePlatform,path);

        public Goal GetNextGoal()
        {
            lock (myLock)
            {
                using (var connection = CreateConnection)
                {
                    var goal =
                        connection.Table<Goal>().Where(g => g.End > DateTime.Now).OrderBy(g => g.End).FirstOrDefault();
                    connection.Commit();
                    connection.Close();
                    return goal;
                }
            }
        }

        public IEnumerable<Goal> All()
        {
            lock (myLock)
            {
                using (var connection = CreateConnection)
                {
                    var goals = connection.Table<Goal>().ToList();
                    connection.Commit();
                    connection.Close();
                    return goals;
                }
            }
        }

        public void Remove(Goal goal)
        {
            lock (myLock)
            {
                using (var connection = CreateConnection)
                {
                    connection.Delete(goal);
                    connection.Commit();
                    connection.Close();
                }
            }
        }

        public Task<IEnumerable<Goal>>  AllAsync()
        {
            return Task.Run(() => All());
        }
    }
}
