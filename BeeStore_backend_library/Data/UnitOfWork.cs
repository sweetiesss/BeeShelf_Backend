using BeeStore_Repository.Models;
using BeeStore_Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Data
{
    public class UnitOfWork : IDisposable
    {
        private BeeStoreDbContext context = new BeeStoreDbContext();
        private GenericRepository<User> userRepo;

        public GenericRepository<User> UserRepo
        {
            get
            {
                if (this.userRepo == null)
                {
                    this.userRepo = new GenericRepository<User>(context);
                }
                return userRepo;
            }
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
