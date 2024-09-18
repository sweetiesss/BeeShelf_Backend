using BeeStore_Repository.Data;
using BeeStore_Repository.Models;
using BeeStore_Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository
{
    public class UnitOfWork : IDisposable
    {
        private BeeStoreDbContext _context;
        private GenericRepository<User> userRepo;

        public UnitOfWork(BeeStoreDbContext context)
        {
            _context = context;
        }

        public GenericRepository<User> UserRepo
        {
            get
            {
                if (userRepo == null)
                {
                    userRepo = new GenericRepository<User>(_context);
                }
                return userRepo;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
