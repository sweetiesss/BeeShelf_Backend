using System.Linq.Expressions;

namespace BeeStore_Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
        Task<T> GetByKeyAsync<TKey>(TKey key, Expression<Func<T, bool>> keySelector);
    }
}
