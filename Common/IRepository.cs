using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model;

namespace Common
{
    public interface IRepository<T> where T : IItem
    {
        Task<bool> AddAsync(T item);
        Task<bool> UpdateAsync(ItemId id, T newValue);
        Task<bool> AddAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(ItemId id);
        Task<bool> RemoveAsync(ItemId id);
        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
    }
}