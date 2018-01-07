using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SavingFor.LPI
{
    public interface IRepository<T>
    {
        void Add(T item);
        void Update(T item);
        T Get(Guid id);
        IEnumerable<T> All();
        Task<IEnumerable<T>> AllAsync();
        void Remove(T item);
    }
}
