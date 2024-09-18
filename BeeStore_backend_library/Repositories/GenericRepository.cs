using BeeStore_Repository.Data;
using BeeStore_Repository.DTO;
using BeeStore_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace BeeStore_Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly BeeStoreDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(BeeStoreDbContext context)
        {
            this._context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task<List<T>> GetAllAsync() => _dbSet.ToListAsync();


        public virtual async Task<IQueryable<T>> GetQueryable()
        {
            IQueryable<T> query = _dbSet;
            return query;
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<T> GetByKeyAsync<TKey>(TKey key, Expression<Func<T, bool>> keySelector)
        {
            return await _dbSet.Where(keySelector).FirstOrDefaultAsync();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _dbSet.CountAsync(filter);
            }
            return await _dbSet.CountAsync();
        }
    }
}
