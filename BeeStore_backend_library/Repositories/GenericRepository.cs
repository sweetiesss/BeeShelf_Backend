using Amazon.S3.Model;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO;
using BeeStore_Repository.Interfaces;
using BeeStore_Repository.Models;
using Microsoft.EntityFrameworkCore;
using NLog.Filters;
using System;
using System.Linq;
using System.Linq.Expressions;
using static Amazon.S3.Util.S3EventNotification;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            
            return await _dbSet.Where(x=> x.IsDeleted.Equals(false)).Where(filter).ToListAsync();
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

        public async Task<T> SingleOrDefaultAsync(
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
    }
}
