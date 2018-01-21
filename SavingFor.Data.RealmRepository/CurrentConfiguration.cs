using Realms;
using System.Linq;
using System.Threading;

namespace SavingFor.Data.RealmRepository
{
    public sealed class CurrentConfiguration
    {
        private static CurrentConfiguration instance;
        private const ulong CurrentSchemaVersion = 2;

        public static CurrentConfiguration Instance()
        {
            if(instance == null)
            {
                Interlocked.CompareExchange(ref instance, new CurrentConfiguration(), null);
            }
            return instance;
        }

        public RealmConfiguration Configuration()
        {
            return new RealmConfiguration
            {
                SchemaVersion = CurrentSchemaVersion,
                MigrationCallback = (migration,oldSchemaVersion) =>
                {
                    var newGoals = migration.NewRealm.All<Goal_r>();
                    var oldGoals = migration.OldRealm.All("Goal_r");
                    for(var i = 0; i < newGoals.Count(); i++)
                    {
                        var oldGoal = oldGoals.ElementAt(i);
                        var newGoal = newGoals.ElementAt(i);

                        if(oldSchemaVersion < 2)
                        {
                            newGoal.Group = string.Empty;
                        }
                    }
                    // Examplecode
                    //var newPeople = migration.NewRealm.All<Person>();
                    //var oldPeople = migration.OldRealm.All("Person");

                    //for (var i = 0; i < newPeople.Count(); i++)
                    //{
                    //    var oldPerson = oldPeople.ElementAt(i);
                    //    var newPerson = newPeople.ElementAt(i);

                    //    // Migrate Person from version 0 to 1: replace FirstName and LastName with FullName
                    //    if (oldSchemaVersion < 1)
                    //    {
                    //        newPerson.FullName = oldPerson.FirstName + " " + oldPerson.LastName;
                    //    }

                    //    // Migrate Person from version 1 to 2: replace Age with Birthday
                    //    if (oldSchemaVersion < 2)
                    //    {
                    //        newPerson.Birthday = DateTimeOffset.Now.AddYears(-(int)oldPerson.Age);
                    //    }
                    //}
                }
            };
        }
    }
}
