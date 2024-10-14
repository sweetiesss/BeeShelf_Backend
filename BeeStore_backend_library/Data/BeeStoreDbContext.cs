using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Data;
public partial class BeeStoreDbContext : DbContext
{


    public BeeStoreDbContext(DbContextOptions<BeeStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseCategory> WarehouseCategories { get; set; }

    public virtual DbSet<WarehouseShipper> WarehouseShippers { get; set; }

    public virtual DbSet<WarehouseStaff> WarehouseStaffs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Inventory");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BoughtDate)
                .HasColumnType("datetime")
                .HasColumnName("bought_date");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.MaxWeight)
                .HasPrecision(10, 2)
                .HasColumnName("max_weight");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.User).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Inventory_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Inventory_ibfk_2");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Order");

            entity.HasIndex(e => e.DeliverBy, "deliver_by");

            entity.HasIndex(e => e.PictureId, "picture_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(70)
                .HasColumnName("cancellation_reason")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CodStatus)
                .HasMaxLength(5)
                .HasColumnName("COD_Status")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.DeliverBy).HasColumnName("deliver_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(10)
                .HasColumnName("order_status")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PictureId).HasColumnName("picture_id");
            entity.Property(e => e.ProductAmount).HasColumnName("product_amount");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ReceiverAddress)
                .HasMaxLength(70)
                .HasColumnName("receiver_address")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(10)
                .HasColumnName("receiver_phone");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.DeliverByNavigation).WithMany(p => p.OrderDeliverByNavigations)
                .HasForeignKey(d => d.DeliverBy)
                .HasConstraintName("Order_ibfk_2");

            entity.HasOne(d => d.Picture).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PictureId)
                .HasConstraintName("Order_ibfk_3");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Order_ibfk_4");

            entity.HasOne(d => d.User).WithMany(p => p.OrderUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Order_ibfk_1");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Package");

            entity.HasIndex(e => e.InventoryId, "inventory_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ProductAmount).HasColumnName("product_amount");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Inventory).WithMany(p => p.Packages)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("Package_ibfk_2");

            entity.HasOne(d => d.Product).WithMany(p => p.Packages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Package_ibfk_1");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Partner");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .HasColumnName("bank_account_number");
            entity.Property(e => e.BankName)
                .HasMaxLength(50)
                .HasColumnName("bank_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CitizenIdentificationNumber)
                .HasMaxLength(50)
                .HasColumnName("citizen_identification_number");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Partners)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Partner_ibfk_1");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Picture");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.PictureLink)
                .HasMaxLength(100)
                .HasColumnName("picture_link");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Product");

            entity.HasIndex(e => e.PictureId, "picture_id");

            entity.HasIndex(e => e.ProductCategoryId, "product_category_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Origin)
                .HasMaxLength(50)
                .HasColumnName("origin")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PictureId).HasColumnName("picture_id");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductCategoryId).HasColumnName("product_category_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.Picture).WithMany(p => p.Products)
                .HasForeignKey(d => d.PictureId)
                .HasConstraintName("Product_ibfk_2");

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategoryId)
                .HasConstraintName("Product_ibfk_3");

            entity.HasOne(d => d.User).WithMany(p => p.Products)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Product_ibfk_1");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Product_Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpireIn).HasColumnName("expire_in");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.TypeDescription)
                .HasMaxLength(100)
                .HasColumnName("type_description")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Request");

            entity.HasIndex(e => e.PackageId, "package_id");

            entity.HasIndex(e => e.SendToInventory, "send_to_inventory");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.RequestType)
                .HasMaxLength(10)
                .HasColumnName("request_type")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.SendToInventory).HasColumnName("send_to_inventory");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Package).WithMany(p => p.Requests)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("Request_ibfk_2");

            entity.HasOne(d => d.SendToInventoryNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.SendToInventory)
                .HasConstraintName("Request_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Request_ibfk_3");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ItemId, "item_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(100)
                .HasColumnName("cancellation_reason")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(50)
                .HasColumnName("transaction_code")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Item).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("Transactions_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Transactions_ibfk_2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.PictureId, "picture_id");

            entity.HasIndex(e => e.RoleId, "role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");
            entity.Property(e => e.PictureId).HasColumnName("picture_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Setting)
                .HasMaxLength(255)
                .HasColumnName("setting")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.Picture).WithMany(p => p.Users)
                .HasForeignKey(d => d.PictureId)
                .HasConstraintName("User_ibfk_1");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("User_ibfk_2");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Warehouse");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Location)
                .HasMaxLength(70)
                .HasColumnName("location")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Size)
                .HasPrecision(10, 2)
                .HasColumnName("size");
        });

        modelBuilder.Entity<WarehouseCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Warehouse_Category");

            entity.HasIndex(e => e.ProductCategoryId, "product_category_id");

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ProductCategoryId).HasColumnName("product_category_id");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.WarehouseCategories)
                .HasForeignKey(d => d.ProductCategoryId)
                .HasConstraintName("Warehouse_Category_ibfk_2");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseCategories)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Warehouse_Category_ibfk_1");
        });

        modelBuilder.Entity<WarehouseShipper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Warehouse_Shipper");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.User).WithMany(p => p.WarehouseShippers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Warehouse_Shipper_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseShippers)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Warehouse_Shipper_ibfk_2");
        });

        modelBuilder.Entity<WarehouseStaff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Warehouse_Staff");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.User).WithMany(p => p.WarehouseStaffs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Warehouse_Staff_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseStaffs)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Warehouse_Staff_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
