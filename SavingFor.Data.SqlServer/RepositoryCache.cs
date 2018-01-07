using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SavingFor.LPI;

namespace SavingFor.Data.SqlServer
{
    public sealed class RepositoryCache<T> : IRepository<T> where T:IEntity
    {
        private readonly IDictionary<int,T> items = new Dictionary<int, T>(); 
        private readonly IRepository<T> decorated;

        public RepositoryCache(IRepository<T> decorated)
        {
            this.decorated = decorated;
        }

        public void Add(T item)
        {
            decorated.Add(item);
            items.Add(item.Id,item);
        }

        public void Update(T item)
        {
            if (items.ContainsKey(item.Id))
            {
                items[item.Id] = item;
            }

            decorated.Update(item);
        }

        public T Get(int id)
        {
            return items.ContainsKey(id) ? items[id] : decorated.Get(id);
        }

        public IEnumerable<T> All()
        {
            if (items.Any())
                return items.Values;

            var all = decorated.All();
            foreach (var item in all)
            {
                items.Add(item.Id,item);
            }

            return items.Values;

        }

        public async Task<IEnumerable<T>> AllAsync()
        {
            if (items.Any()) return await Task.Run(() => items.Values);
            var all = await decorated.AllAsync();
            foreach (var item in all)
            {
                items.Add(item.Id,item);
            }
            return await Task.Run(() => items.Values);
        }

        public void Remove(T item)
        {
            if (items.ContainsKey(item.Id))
            {
                items.Remove(item.Id);
            }
            decorated.Remove(item);
        }
    }
}
