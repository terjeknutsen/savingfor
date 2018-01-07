using System.Threading;
using SavingFor.Data.RealmRepository;
using SavingFor.Domain.Model;
using SavingFor.LPI;
using System.Security;

namespace SavingFor.AndroidClient.Factories
{
    class RepositoryFactory
    {
        public static RepositoryFactory GetSingleton()
        {
            if (factory != null) return factory;
            var tmp = new RepositoryFactory();
            Interlocked.CompareExchange(ref factory, tmp, null);
            return factory;
        }
        private static IRepository<Goal> goalRepository;
        private static RepositoryFactory factory;

        public IRepository<Goal> GetRepository()
        {
            return goalRepository ?? (goalRepository = CreateCacheRepository());
        }
       [SecuritySafeCritical]
        private static IRepository<Goal> CreateCacheRepository()
        {
            var repository = new GoalRepositoryRealm();
            return repository;
        }
    }
}