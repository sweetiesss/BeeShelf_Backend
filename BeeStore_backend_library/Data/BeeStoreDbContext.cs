using System;
using System.Collections.Generic;
using BeeStore_Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace BeeStore_Repository.Data;

public partial class BeeStoreDbContext : DbContext
{

    public BeeStoreDbContext(DbContextOptions<BeeStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoryType> CategoryTypes { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseShipper> WarehouseShippers { get; set; }

    public virtual DbSet<WarehouseStaff> WarehouseStaffs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<CategoryType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Category_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpireIn).HasColumnName("expire_in");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.TypeDescription)
                .HasColumnType("text")
                .HasColumnName("type_description");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Inventory");

            entity.HasIndex(e => e.PartnerEmail, "partner_email");

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.Length).HasColumnName("length");
            entity.Property(e => e.MaxWeight).HasColumnName("max_weight");
            entity.Property(e => e.PartnerEmail).HasColumnName("partner_email");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.PartnerEmailNavigation).WithMany(p => p.Inventories)
                .HasPrincipalKey(p => p.UserEmail)
                .HasForeignKey(d => d.PartnerEmail)
                .HasConstraintName("Inventory_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Inventory_ibfk_2");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Order");

            entity.HasIndex(e => e.PartnerEmail, "partner_email");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.DeliverBy)
                .HasMaxLength(100)
                .HasColumnName("deliver_by");
            entity.Property(e => e.IsCod)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_COD");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.OrderDeliveryStatus)
                .HasMaxLength(20)
                .HasColumnName("order_delivery_status");
            entity.Property(e => e.OrderProcessStatus)
                .HasMaxLength(20)
                .HasColumnName("order_process_status");
            entity.Property(e => e.PartnerEmail).HasColumnName("partner_email");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .HasColumnName("picture");
            entity.Property(e => e.ProductAmount).HasColumnName("product_amount");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductPrice)
                .HasPrecision(10, 2)
                .HasColumnName("product_price");
            entity.Property(e => e.ReceiverAddress)
                .HasMaxLength(255)
                .HasColumnName("receiver_address");
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(20)
                .HasColumnName("receiver_phone");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");

            entity.HasOne(d => d.PartnerEmailNavigation).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.UserEmail)
                .HasForeignKey(d => d.PartnerEmail)
                .HasConstraintName("Order_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Order_ibfk_2");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Package");

            entity.HasIndex(e => e.InventoryId, "inventory_id");

            entity.HasIndex(e => e.PartnerEmail, "partner_email");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.Length).HasColumnName("length");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PartnerEmail).HasColumnName("partner_email");
            entity.Property(e => e.ProductAmount).HasColumnName("product_amount");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.Inventory).WithMany(p => p.Packages)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("Package_ibfk_3");

            entity.HasOne(d => d.PartnerEmailNavigation).WithMany(p => p.Packages)
                .HasPrincipalKey(p => p.UserEmail)
                .HasForeignKey(d => d.PartnerEmail)
                .HasConstraintName("Package_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Packages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Package_ibfk_2");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Partner");

            entity.HasIndex(e => e.CitizenIdentificationNumber, "citizen_identification_number").IsUnique();

            entity.HasIndex(e => e.UserEmail, "user_email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .HasColumnName("bank_account_number");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .HasColumnName("bank_name");
            entity.Property(e => e.CitizenIdentificationNumber)
                .HasMaxLength(50)
                .HasColumnName("citizen_identification_number");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.UserEmail).HasColumnName("user_email");

            entity.HasOne(d => d.UserEmailNavigation).WithOne(p => p.Partner)
                .HasPrincipalKey<User>(p => p.Email)
                .HasForeignKey<Partner>(d => d.UserEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Partner_ibfk_1");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Product");

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("Product_ibfk_1");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_image");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.ImageLink)
                .HasMaxLength(255)
                .HasColumnName("image_link");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_image_ibfk_1");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Request");

            entity.HasIndex(e => e.PackageId, "package_id");

            entity.HasIndex(e => e.SendToInventoryId, "send_to_inventory_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.SendToInventoryId).HasColumnName("send_to_inventory_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Package).WithMany(p => p.Requests)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("Request_ibfk_1");

            entity.HasOne(d => d.SendToInventory).WithMany(p => p.Requests)
                .HasForeignKey(d => d.SendToInventoryId)
                .HasConstraintName("Request_ibfk_2");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.RoleId, "role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .HasColumnName("picture");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("User_ibfk_1");
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
                .HasColumnName("isDeleted");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .HasColumnName("picture");
            entity.Property(e => e.Size).HasColumnName("size");
        });

        modelBuilder.Entity<WarehouseShipper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Warehouse_Shipper");

            entity.HasIndex(e => e.UserEmail, "user_email").IsUnique();

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UserEmail).HasColumnName("user_email");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.UserEmailNavigation).WithOne(p => p.WarehouseShipper)
                .HasPrincipalKey<User>(p => p.Email)
                .HasForeignKey<WarehouseShipper>(d => d.UserEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Warehouse_Shipper_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseShippers)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Warehouse_Shipper_ibfk_2");
        });

        modelBuilder.Entity<WarehouseStaff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Warehouse_Staff");

            entity.HasIndex(e => e.UserEmail, "user_email").IsUnique();

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("isDeleted");
            entity.Property(e => e.UserEmail).HasColumnName("user_email");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.UserEmailNavigation).WithOne(p => p.WarehouseStaff)
                .HasPrincipalKey<User>(p => p.Email)
                .HasForeignKey<WarehouseStaff>(d => d.UserEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Warehouse_Staff_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseStaffs)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Warehouse_Staff_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
