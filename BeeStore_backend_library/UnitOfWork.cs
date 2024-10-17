using BeeStore_Repository.Data;
using BeeStore_Repository.Models;
using BeeStore_Repository.Repositories;

namespace BeeStore_Repository
{
    public class UnitOfWork : IDisposable
    {
        private BeeStoreDbContext _context;
        private GenericRepository<User> userRepo;
        private GenericRepository<Role> roleRepo;
        private GenericRepository<Picture> pictureRepo;
        private GenericRepository<Partner> partnerRepo;
        private GenericRepository<Warehouse> warehouseRepo;
        private GenericRepository<Inventory> inventoryRepo;
        private GenericRepository<Product> productRepo;
        private GenericRepository<ProductCategory> productCategoryRepo;
        private GenericRepository<Package> packageRepo;
        private GenericRepository<WarehouseCategory> warehouseCategoryRepo;
        private GenericRepository<WarehouseShipper> warehouseShipperRepo;
        private GenericRepository<WarehouseStaff> warehouseStaffRepo;
        private GenericRepository<Request> requestRepo;
        private GenericRepository<Order> orderRepo;

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
                    _context.ChangeTracker.LazyLoadingEnabled = false;
                }
                return userRepo;
            }
        }

        public GenericRepository<Role> RoleRepo
        {
            get
            {
                if (roleRepo == null)
                {
                    roleRepo = new GenericRepository<Role>(_context);
                }
                return roleRepo;
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

        public GenericRepository<WarehouseCategory> WarehouseCategoryRepo
        {
            get
            {
                if (warehouseCategoryRepo == null)
                {
                    warehouseCategoryRepo = new GenericRepository<WarehouseCategory>(_context);
                }
                return warehouseCategoryRepo;
            }
        }

        public GenericRepository<WarehouseShipper> WarehouseShipperRepo
        {
            get
            {
                if (warehouseShipperRepo == null)
                {
                    warehouseShipperRepo = new GenericRepository<WarehouseShipper>(_context);
                }
                return warehouseShipperRepo;
            }
        }

        public GenericRepository<WarehouseStaff> WarehouseStaffRepo
        {
            get
            {
                if (warehouseStaffRepo == null)
                {
                    warehouseStaffRepo = new GenericRepository<WarehouseStaff>(_context);
                }
                return warehouseStaffRepo;
            }
        }

        public GenericRepository<Request> RequestRepo
        {
            get
            {
                if (requestRepo == null)
                {
                    requestRepo = new GenericRepository<Request>(_context);
                }
                return requestRepo;
            }
        }

        public GenericRepository<Order> OrderRepo
        {
            get
            {
                if (orderRepo == null)
                {
                    orderRepo = new GenericRepository<Order>(_context);
                }
                return orderRepo;
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
