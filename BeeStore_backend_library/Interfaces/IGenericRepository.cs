using BeeStore_Repository.Models;
using System.Linq.Expressions;

namespace BeeStore_Repository.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        //Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<IEnumerable<T>> GetFiltered(Expression<Func<T, bool>> filter);
        //Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> SingleOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> includes = null);

        Task<List<T>> GetQueryable(Func<IQueryable<T>, IQueryable<T>> includes = null);

        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entities);
        void Update(T entity);
        void UpdateRange(List<T> entities);
        void SoftDelete(T entity);
        void SoftDeleteRange(List<T> entities);
        void HardDelete(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> GetListAsync(Expression<Func<T, bool>> filter = null, 
                                                      Func<IQueryable<T>, IQueryable<T>> includes = null,
                                                      string sortBy = null,
                                                      bool descending = false, 
                                                      string searchTerm = null,
                                                      params Expression<Func<T, string>>[] searchProperties);
        //use in generic only
        IQueryable<T> SortBy(IQueryable<T> query, string sortBy, bool descending = false);
        IQueryable<T> SearchBy(IQueryable<T> query, string searchTerm, params Expression<Func<T, string>>[] properties);
        IQueryable<T> FilterBy(Expression<Func<T, bool>> predicate);
    }
}
