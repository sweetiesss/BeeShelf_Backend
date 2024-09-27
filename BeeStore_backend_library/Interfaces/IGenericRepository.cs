using BeeStore_Repository.Models;
using System.Linq.Expressions;

namespace BeeStore_Repository.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        //Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entities);
        void Update(T entity);
        void UpdateRange(List<T> entities);
        void SoftDelete(T entity);
        void SoftDeleteRange(List<T> entities);
        void HardDelete(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
        Task<T> GetByKeyAsync<TKey>(TKey key, Expression<Func<T, bool>> keySelector);
    }
}
