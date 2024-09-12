using System;
using System.Collections.Generic;
using System.Configuration;
using BeeStore_Repository.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BeeStore_Repository.Data;

public partial class BeeStoreDbContext : DbContext
{
    public BeeStoreDbContext()
    {
    }

    public BeeStoreDbContext(DbContextOptions<BeeStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseShipper> WarehouseShippers { get; set; }

    public virtual DbSet<WarehouseStaff> WarehouseStaffs { get; set; }


    //
    // Pay absolutely no attention here.
    // Nothing to see here.
    // Just ignore this part, do NOT modify this.
    //
    //private string RetrieveApiKey()
    //{
    //    var builder = new ConfigurationBuilder()
    //    .SetBasePath(Directory.GetCurrentDirectory())
    //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    //    IConfigurationRoot _configuration = builder.Build();

    //    var keyVaultURL = _configuration.GetValue<string>(AuthConstant.KeyVaultUrl);
    //    var keyVaultClientId = _configuration.GetValue<string>(AuthConstant.ClientId);
    //    var keyVaultClientSecret = _configuration.GetValue<string>(AuthConstant.ClientSecret);
    //    var keyVaultDirectoryId = _configuration.GetValue<string>(AuthConstant.DirectoryId);


    //    if (string.IsNullOrEmpty(keyVaultURL) ||
    //        string.IsNullOrEmpty(keyVaultClientId) ||
    //        string.IsNullOrEmpty(keyVaultClientSecret) ||
    //        string.IsNullOrEmpty(keyVaultDirectoryId))
    //    {
    //        throw new InvalidOperationException("Key Vault configuration values are missing.");
    //    }

    //    var credential = new ClientSecretCredential(
    //        keyVaultDirectoryId,
    //        keyVaultClientId,
    //        keyVaultClientSecret
    //    );

    //    var client = new SecretClient(new Uri(keyVaultURL), credential);

    //    var secretResponse = client.GetSecret("BeeStore-Apikey");

    //    return secretResponse.Value.Value;
    //}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfigurationRoot configuration = builder.Build();

        optionsBuilder.UseMySQL(configuration.GetConnectionString("DatabaseConnection"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateToExpire).HasColumnName("date_to_expire");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Image");

            entity.HasIndex(e => e.ProductId, "Image_ibfk_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.ImageLink)
                .HasMaxLength(255)
                .HasColumnName("image_link");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UpdateDate).HasColumnName("update_date");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Inventory");

            entity.HasIndex(e => e.PartnerEmail, "Inventory_ibfk_1");

            entity.HasIndex(e => e.WarehouseId, "Inventory_ibfk_2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AmountOfItem).HasColumnName("amount_of_item");
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.PartnerEmail).HasColumnName("partner_email");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.PartnerEmailNavigation).WithMany(p => p.Inventories)
                .HasPrincipalKey(p => p.UserEmail)
                .HasForeignKey(d => d.PartnerEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Inventory_ibfk_1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Inventory_ibfk_2");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Order");

            entity.HasIndex(e => e.PartnerEmail, "Order_ibfk_1");

            entity.HasIndex(e => e.DeliverBy, "deliver_by");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("create_date");
            entity.Property(e => e.DeliverBy)
                .HasColumnName("deliver_by")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IsCod)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_COD");
            entity.Property(e => e.OrderDeliveryStatus)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','in_transit','delivered')")
                .HasColumnName("order_delivery_status");
            entity.Property(e => e.OrderProcessStatus)
                .HasDefaultValueSql("'0'")
                .HasColumnName("order_process_status");
            entity.Property(e => e.PartnerEmail).HasColumnName("partner_email");
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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Order_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Order_ibfk_2");
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
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.UpdateDate).HasColumnName("update_date");
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

            entity.HasIndex(e => e.Barcode, "barcode").IsUnique();

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.HasIndex(e => e.InventoryId, "inventory_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Barcode)
                .HasMaxLength(50)
                .HasColumnName("barcode");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasColumnName("status");
            entity.Property(e => e.Volume).HasColumnName("volume");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("Product_ibfk_1");

            entity.HasOne(d => d.Inventory).WithMany(p => p.Products)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("Product_ibfk_2");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnName("id");
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
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
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
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Size).HasColumnName("size");
        });

        modelBuilder.Entity<WarehouseShipper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Warehouse_Shipper");

            entity.HasIndex(e => e.UserEmail, "user_email").IsUnique();

            entity.HasIndex(e => e.WarehouseId, "warehouse_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
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
