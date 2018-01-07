using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Realms;
using SavingFor.Domain.Model;
using SavingFor.LPI;

namespace SavingFor.Data.RealmRepository
{
    public sealed class GoalRepositoryRealm : IRepository<Goal>
    {
        public void Add(Goal item)
        {
            var realm = GetRealm();

            realm.Write(() =>
            {
                var instance = new Goal_r();
                MapGoal(item, instance);
                realm.Add(instance);
            });
        }
        public void Update(Goal item)
        {
            var realm = GetRealm();
            var instance = realm.All<Goal_r>().First(g => g.Id == item.Id.ToString());
            realm.Write(() =>
            {
                MapGoal(item,instance);
            });
            realm.Refresh();
        }

        public Goal Get(Guid id)
        {
            var realm = GetRealm();
            var instance = realm.All<Goal_r>().First(g => g.Id == id.ToString());
            return MapFromRealm(instance);
        }

        public IEnumerable<Goal> All()
        {
            var realm = GetRealm();
            realm.Refresh();
            var allInstances = realm.All<Goal_r>();
            var goals = new List<Goal>();

            foreach (var allInstance in allInstances)
            {
                goals.Add(MapFromRealm(allInstance));
            }

            return goals;

        }

        public Task<IEnumerable<Goal>> AllAsync()
        {
            return Task.Run(() => All());
        }

        public void Remove(Goal item)
        {
            var id = item.Id.ToString();
            Remove(id);
        }

        public void Remove(string id)
        {
            var realm = GetRealm();
            realm.Write(() =>
            {
                var instance = realm.All<Goal_r>().First(g => g.Id == id.ToString());
                realm.Remove(instance);
            });
        }

        private static void MapGoal(Goal item, Goal_r instance)
        {
            instance.Id = item.Id.ToString();
            instance.Image = item.Image;
            instance.Amount = (double) item.Amount;
            instance.Start = new DateTimeOffset(item.Start);
            instance.End = new DateTimeOffset(item.End);
            instance.Name = item.Name;
            instance.ProgressPerMinute = (double) item.ProgressPerMinute;
        }

        private static Goal MapFromRealm(Goal_r instance)
        {
            return new Goal
            {
                Id = new Guid(instance.Id),
                Name = instance.Name,
                Image = instance.Image,
                Amount = (decimal)instance.Amount,
                Start = instance.Start.DateTime,
                End = instance.End.DateTime,
                ProgressPerMinute = (decimal)instance.ProgressPerMinute
            };
        }
        private static Realm GetRealm()
        {
            return Realm.GetInstance(CurrentConfiguration.Instance().Configuration());
        }
    }

    public class Goal_r : RealmObject
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public string Name { get; set; }
        public double ProgressPerMinute { get; set; }
    }
}
