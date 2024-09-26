﻿using System;
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

            entity.HasIndex(e => e.PartnerEmail, "partner_email");

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
            entity.Property(e => e.PartnerEmail).HasColumnName("partner_email");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.PartnerEmailNavigation).WithMany(p => p.Inventories)
                .HasPrincipalKey(p => p.Email)
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

            entity.HasIndex(e => e.PictureId, "picture_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CancellationReason)
                .HasColumnType("text")
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CodStatus)
                .HasMaxLength(15)
                .HasColumnName("COD_Status");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.DeliverBy)
                .HasColumnType("datetime")
                .HasColumnName("deliver_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(15)
                .HasColumnName("order_status");
            entity.Property(e => e.PartnerEmail).HasColumnName("partner_email");
            entity.Property(e => e.PictureId).HasColumnName("picture_id");
            entity.Property(e => e.ProductAmount).HasColumnName("product_amount");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ReceiverAddress)
                .HasColumnType("text")
                .HasColumnName("receiver_address");
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(50)
                .HasColumnName("receiver_phone");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");

            entity.HasOne(d => d.PartnerEmailNavigation).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.Email)
                .HasForeignKey(d => d.PartnerEmail)
                .HasConstraintName("Order_ibfk_1");

            entity.HasOne(d => d.Picture).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PictureId)
                .HasConstraintName("Order_ibfk_2");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Order_ibfk_3");
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
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

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

            entity.HasIndex(e => e.UserEmail, "user_email");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(255)
                .HasColumnName("bank_account_number");
            entity.Property(e => e.BankName)
                .HasMaxLength(255)
                .HasColumnName("bank_name");
            entity.Property(e => e.CitizenIdentificationNumber)
                .HasMaxLength(255)
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
            entity.Property(e => e.UserEmail).HasColumnName("user_email");

            entity.HasOne(d => d.UserEmailNavigation).WithMany(p => p.Partners)
                .HasPrincipalKey(p => p.Email)
                .HasForeignKey(d => d.UserEmail)
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
                .HasColumnType("text")
                .HasColumnName("picture_link");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Product");

            entity.HasIndex(e => e.PartnerEmail, "partner_email");

            entity.HasIndex(e => e.PictureId, "picture_id");

            entity.HasIndex(e => e.ProductCategoryId, "product_category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Origin)
                .HasMaxLength(255)
                .HasColumnName("origin");
            entity.Property(e => e.PartnerEmail).HasColumnName("partner_email");
            entity.Property(e => e.PictureId).HasColumnName("picture_id");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductAmount).HasColumnName("product_amount");
            entity.Property(e => e.ProductCategoryId).HasColumnName("product_category_id");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.PartnerEmailNavigation).WithMany(p => p.Products)
                .HasPrincipalKey(p => p.Email)
                .HasForeignKey(d => d.PartnerEmail)
                .HasConstraintName("Product_ibfk_1");

            entity.HasOne(d => d.Picture).WithMany(p => p.Products)
                .HasForeignKey(d => d.PictureId)
                .HasConstraintName("Product_ibfk_2");

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategoryId)
                .HasConstraintName("Product_ibfk_3");
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
                .HasColumnType("text")
                .HasColumnName("type_description");
            entity.Property(e => e.TypeName)
                .HasMaxLength(255)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Request");

            entity.HasIndex(e => e.PackageId, "package_id");

            entity.HasIndex(e => e.SendToInventory, "send_to_inventory");

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
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.SendToInventory).HasColumnName("send_to_inventory");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasColumnName("status");

            entity.HasOne(d => d.Package).WithMany(p => p.Requests)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("Request_ibfk_2");

            entity.HasOne(d => d.SendToInventoryNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.SendToInventory)
                .HasConstraintName("Request_ibfk_1");
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
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.BuyerEmail, "buyer_email");

            entity.HasIndex(e => e.ItemId, "item_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BuyerEmail).HasColumnName("buyer_email");
            entity.Property(e => e.CancellationReason)
                .HasColumnType("text")
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasColumnName("status");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(255)
                .HasColumnName("transaction_code");

            entity.HasOne(d => d.BuyerEmailNavigation).WithMany(p => p.Transactions)
                .HasPrincipalKey(p => p.Email)
                .HasForeignKey(d => d.BuyerEmail)
                .HasConstraintName("Transactions_ibfk_2");

            entity.HasOne(d => d.Item).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("Transactions_ibfk_1");
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
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.PictureId).HasColumnName("picture_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Setting)
                .HasMaxLength(255)
                .HasColumnName("setting");

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
                .HasColumnType("text")
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
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

            entity.HasIndex(e => e.UserEmail, "user_email");

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasColumnName("status");
            entity.Property(e => e.UserEmail).HasColumnName("user_email");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.UserEmailNavigation).WithMany(p => p.WarehouseShippers)
                .HasPrincipalKey(p => p.Email)
                .HasForeignKey(d => d.UserEmail)
                .HasConstraintName("Warehouse_Shipper_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseShippers)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Warehouse_Shipper_ibfk_2");
        });

        modelBuilder.Entity<WarehouseStaff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Warehouse_Staff");

            entity.HasIndex(e => e.UserEmail, "user_email");

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.UserEmail).HasColumnName("user_email");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.UserEmailNavigation).WithMany(p => p.WarehouseStaffs)
                .HasPrincipalKey(p => p.Email)
                .HasForeignKey(d => d.UserEmail)
                .HasConstraintName("Warehouse_Staff_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseStaffs)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("Warehouse_Staff_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

