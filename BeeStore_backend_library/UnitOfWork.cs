using BeeStore_Repository.Data;
using BeeStore_Repository.Models;
using BeeStore_Repository.Repositories;

namespace BeeStore_Repository
{
    public class UnitOfWork : IDisposable
    {
        private BeeStoreDbContext _context;
        private GenericRepository<Employee> employeeRepo;
        private GenericRepository<OcopPartner> ocopPartnerRepo;
        private GenericRepository<Lot> lotRepo;
        private GenericRepository<Wallet> walletRepo;
        private GenericRepository<OrderDetail> orderDetailRepo;
        private GenericRepository<Batch> batchRepo;
        private GenericRepository<BatchDelivery> batchDeliveryRepo;
        private GenericRepository<Transaction> transactionRepo;

        private GenericRepository<Role> roleRepo;
        private GenericRepository<Picture> pictureRepo;
        private GenericRepository<Partner> partnerRepo;
        private GenericRepository<Warehouse> warehouseRepo;
        private GenericRepository<Inventory> inventoryRepo;
        private GenericRepository<Product> productRepo;
        private GenericRepository<ProductCategory> productCategoryRepo;
        private GenericRepository<WarehouseShipper> warehouseShipperRepo;
        private GenericRepository<WarehouseStaff> warehouseStaffRepo;
        private GenericRepository<Request> requestRepo;
        private GenericRepository<Order> orderRepo;

        public UnitOfWork(BeeStoreDbContext context)
        {
            _context = context;
        }

        public GenericRepository<Batch> BatchRepo
        {
            get
            {
                if (batchRepo == null)
                {
                    batchRepo = new GenericRepository<Batch>(_context);
                }
                return batchRepo;
            }
        }

        public GenericRepository<BatchDelivery> BatchDeliveryRepo
        {
            get
            {
                if (batchDeliveryRepo == null)
                {
                    batchDeliveryRepo = new GenericRepository<BatchDelivery>(_context);
                }
                return batchDeliveryRepo;
            }
        }

        public GenericRepository<Transaction> TransactionRepo
        {
            get
            {
                if (transactionRepo == null)
                {
                    transactionRepo = new GenericRepository<Transaction>(_context);
                }
                return transactionRepo;
            }
        }

        public GenericRepository<Employee> EmployeeRepo
        {
            get
            {
                if (employeeRepo == null)
                {
                    employeeRepo = new GenericRepository<Employee>(_context);
                    _context.ChangeTracker.LazyLoadingEnabled = false;
                }
                return employeeRepo;
            }
        }

        public GenericRepository<OrderDetail> OrderDetailRepo
        {
            get
            {
                if (orderDetailRepo == null)
                {
                    orderDetailRepo = new GenericRepository<OrderDetail>(_context);
                    _context.ChangeTracker.LazyLoadingEnabled = false;
                }
                return orderDetailRepo;
            }
        }

        public GenericRepository<OcopPartner> OcopPartnerRepo
        {
            get
            {
                if (ocopPartnerRepo == null)
                {
                    ocopPartnerRepo = new GenericRepository<OcopPartner>(_context);
                    _context.ChangeTracker.LazyLoadingEnabled = false;
                }
                return ocopPartnerRepo;
            }
        }

        public GenericRepository<Lot> LotRepo
        {
            get
            {
                if (lotRepo == null)
                {
                    lotRepo = new GenericRepository<Lot>(_context);
                }
                return lotRepo;
            }
        }

        public GenericRepository<Wallet> WalletRepo
        {
            get
            {
                if (walletRepo == null)
                {
                    walletRepo = new GenericRepository<Wallet>(_context);
                }
                return walletRepo;
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
