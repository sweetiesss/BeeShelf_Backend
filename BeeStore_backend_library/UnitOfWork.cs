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
        private GenericRepository<Picture> pictureRepo;
        private GenericRepository<Partner> partnerRepo;
        private GenericRepository<Warehouse> warehouseRepo;
        private GenericRepository<Inventory> inventoryRepo;
        private GenericRepository<Product> productRepo;
        private GenericRepository<ProductCategory> productCategoryRepo;
        private GenericRepository<Package> packageRepo;

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

        public GenericRepository<Picture> PictureRepo
        {
            get
            {
                if (pictureRepo == null)
                {
                    pictureRepo = new GenericRepository<Picture>(_context);
                }
                return pictureRepo;
            }
        }

        public GenericRepository<Partner> PartnerRepo
        {
            get
            {
                if (partnerRepo == null)
                {
                    partnerRepo = new GenericRepository<Partner>(_context);
                }
                return partnerRepo;
            }
        }

        public GenericRepository<Warehouse> WarehouseRepo
        {
            get
            {
                if (warehouseRepo == null)
                {
                    warehouseRepo = new GenericRepository<Warehouse>(_context);
                }
                return warehouseRepo;
            }
        }

        public GenericRepository<Inventory> InventoryRepo
        {
            get
            {
                if (inventoryRepo == null)
                {
                    inventoryRepo = new GenericRepository<Inventory>(_context);
                }
                return inventoryRepo;
            }
        }

        public GenericRepository<Product> ProductRepo
        {
            get
            {
                if (productRepo == null)
                {
                    productRepo = new GenericRepository<Product>(_context);
                }
                return productRepo;
            }
        }

        public GenericRepository<ProductCategory> ProductCategoryRepo
        {
            get
            {
                if (productCategoryRepo == null)
                {
                    productCategoryRepo = new GenericRepository<ProductCategory>(_context);
                }
                return productCategoryRepo;
            }
        }

        public GenericRepository<Package> PackageRepo
        {
            get
            {
                if (packageRepo == null)
                {
                    packageRepo = new GenericRepository<Package>(_context);
                }
                return packageRepo;
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
