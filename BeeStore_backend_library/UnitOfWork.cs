using BeeStore_Repository.Data;
using BeeStore_Repository.Interfaces;
using BeeStore_Repository.Models;
using BeeStore_Repository.Repositories;

namespace BeeStore_Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<OcopCategory> OcopCategoryRepo { get; }
        IGenericRepository<Category> CategoryRepo {  get; }
        IGenericRepository<Employee> EmployeeRepo { get; }
        IGenericRepository<OcopPartner> OcopPartnerRepo { get; }
        IGenericRepository<Lot> LotRepo { get; }
        IGenericRepository<Wallet> WalletRepo { get; }
        IGenericRepository<OrderDetail> OrderDetailRepo { get; }
        IGenericRepository<Batch> BatchRepo { get; }
        IGenericRepository<BatchDelivery> BatchDeliveryRepo { get; }
        IGenericRepository<Transaction> TransactionRepo { get; }
        IGenericRepository<OrderFee> OrderFeeRepo { get; }
        IGenericRepository<Vehicle> VehicleRepo { get; }
        IGenericRepository<Role> RoleRepo { get; }
        IGenericRepository<Warehouse> WarehouseRepo { get; }
        IGenericRepository<Inventory> InventoryRepo { get; }
        IGenericRepository<Product> ProductRepo { get; }
        IGenericRepository<ProductCategory> ProductCategoryRepo { get; }
        IGenericRepository<WarehouseShipper> WarehouseShipperRepo { get; }
        IGenericRepository<WarehouseStaff> WarehouseStaffRepo { get; }
        IGenericRepository<Request> RequestRepo { get; }
        IGenericRepository<Order> OrderRepo { get; }

        Task SaveAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private BeeStoreDbContext _context;
        private IGenericRepository<OcopCategory> ocopCategoryRepo;
        private IGenericRepository<Category> categoryRepo;
        private IGenericRepository<Employee> employeeRepo;
        private IGenericRepository<OcopPartner> ocopPartnerRepo;
        private IGenericRepository<Lot> lotRepo;
        private IGenericRepository<Wallet> walletRepo;
        private IGenericRepository<OrderDetail> orderDetailRepo;
        private IGenericRepository<Batch> batchRepo;
        private IGenericRepository<BatchDelivery> batchDeliveryRepo;
        private IGenericRepository<Transaction> transactionRepo;
        private IGenericRepository<OrderFee> orderFeeRepo;
        private IGenericRepository<Vehicle> vehicleRepo;

        private IGenericRepository<Role> roleRepo;
        private IGenericRepository<Warehouse> warehouseRepo;
        private IGenericRepository<Inventory> inventoryRepo;
        private IGenericRepository<Product> productRepo;
        private IGenericRepository<ProductCategory> productCategoryRepo;
        private IGenericRepository<WarehouseShipper> warehouseShipperRepo;
        private IGenericRepository<WarehouseStaff> warehouseStaffRepo;
        private IGenericRepository<Request> requestRepo;
        private IGenericRepository<Order> orderRepo;

        public UnitOfWork(BeeStoreDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Category> CategoryRepo
        {
            get
            {
                if (categoryRepo == null)
                {
                    categoryRepo = new GenericRepository<Category>(_context);
                }
                return categoryRepo;
            }
        }

        public IGenericRepository<OcopCategory> OcopCategoryRepo
        {
            get
            {
                if (ocopCategoryRepo == null)
                {
                    ocopCategoryRepo = new GenericRepository<OcopCategory>(_context);
                }
                return ocopCategoryRepo;
            }
        }

        public IGenericRepository<Batch> BatchRepo
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

        public IGenericRepository<Vehicle> VehicleRepo
        {
            get
            {
                if (vehicleRepo == null)
                {
                    vehicleRepo = new GenericRepository<Vehicle>(_context);
                }
                return vehicleRepo;
            }
        }

        public IGenericRepository<BatchDelivery> BatchDeliveryRepo
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

        public IGenericRepository<OrderFee> OrderFeeRepo
        {
            get
            {
                if (orderFeeRepo == null)
                {
                    orderFeeRepo = new GenericRepository<OrderFee>(_context);
                }
                return orderFeeRepo;
            }
        }

        public IGenericRepository<Transaction> TransactionRepo
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

        public IGenericRepository<Employee> EmployeeRepo
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

        public IGenericRepository<OrderDetail> OrderDetailRepo
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

        public IGenericRepository<OcopPartner> OcopPartnerRepo
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

        public IGenericRepository<Lot> LotRepo
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

        public IGenericRepository<Wallet> WalletRepo
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


        public IGenericRepository<Role> RoleRepo
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


        public IGenericRepository<Warehouse> WarehouseRepo
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

        public IGenericRepository<Inventory> InventoryRepo
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

        public IGenericRepository<Product> ProductRepo
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

        public IGenericRepository<ProductCategory> ProductCategoryRepo
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



        public IGenericRepository<WarehouseShipper> WarehouseShipperRepo
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

        public IGenericRepository<WarehouseStaff> WarehouseStaffRepo
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

        public IGenericRepository<Request> RequestRepo
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

        public IGenericRepository<Order> OrderRepo
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
