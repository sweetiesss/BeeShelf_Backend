using BeeStore_Repository.Data;
using BeeStore_Repository.Interfaces;
using BeeStore_Repository.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BeeStore_Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
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
            return (await _dbSet.FindAsync(id))!;
        }

        public Task<List<T>> GetAllAsync() => _dbSet.Where(x => x.IsDeleted.Equals(false)).ToListAsync();

        public async Task<IEnumerable<T>> GetFiltered(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(x => x.IsDeleted.Equals(false)).Where(filter).ToListAsync();
        }



        public async Task<List<T>> GetQueryable(Func<IQueryable<T>, IQueryable<T>> includes = null)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                query = includes(query);
            }

            return await query.ToListAsync();
        }

        //public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        //{
        //    return await _dbSet.SingleOrDefaultAsync(predicate);
        //}

        public virtual async Task<T> SingleOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> includes = null)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            return (await query.Where(x => x.IsDeleted.Equals(false)).SingleOrDefaultAsync(predicate))!;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(List<T> entities)
        {

            await _dbSet.AddRangeAsync(entities);
        }


        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRange(List<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }


        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return (await _dbSet.Where(x => x.IsDeleted.Equals(false)).FirstOrDefaultAsync(predicate))!;
        }


        public void SoftDelete(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }

        public void SoftDeleteRange(List<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
            }
            _dbSet.UpdateRange(entities);
        }



        public void HardDelete(T entity)
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

        public IQueryable<T> SortBy(IQueryable<T> query, string sortBy, bool descending = false)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var result = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { query.ElementType, property.Type },
                query.Expression,
                lambda
            );

            return query.Provider.CreateQuery<T>(result);
        }

        public IQueryable<T> SearchBy(IQueryable<T> query, string searchTerm, params Expression<Func<T, string>>[] properties)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");

            Expression? combinedExpression = null;
            foreach (var propertyExpression in properties)
            {
                var property = Expression.Invoke(propertyExpression, parameter);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var searchExpression = Expression.Call(property, containsMethod!, Expression.Constant(searchTerm));

                combinedExpression = combinedExpression == null
                    ? searchExpression
                    : Expression.OrElse(combinedExpression, searchExpression);
            }

            if (combinedExpression == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            return query.Where(lambda);
        }

        public IQueryable<T> FilterBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<List<T>> GetListAsync(
       Expression<Func<T, bool>> filter = null,
       Func<IQueryable<T>, IQueryable<T>> includes = null,
       string sortBy = null,
       bool descending = false,
       string searchTerm = null,
       params Expression<Func<T, string>>[] searchProperties)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = FilterBy(filter);
            }

            if (!string.IsNullOrEmpty(searchTerm) && searchProperties.Length > 0)
            {
                query = SearchBy(query, searchTerm, searchProperties);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                query = SortBy(query, sortBy, descending);
            }

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
        }
    }
}
