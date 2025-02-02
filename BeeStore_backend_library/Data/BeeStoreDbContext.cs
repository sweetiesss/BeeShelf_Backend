﻿using BeeStore_Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace BeeStore_Repository.Data;


public partial class BeeStoreDbContext : DbContext
{

    public BeeStoreDbContext(DbContextOptions<BeeStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<DeliveryZone> DeliveryZones { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<ExportFee> ExportFees { get; set; }

    public virtual DbSet<Lot> Lots { get; set; }

    public virtual DbSet<MoneyTransfer> MoneyTransfers { get; set; }

    public virtual DbSet<OcopCategory> OcopCategories { get; set; }

    public virtual DbSet<OcopPartner> OcopPartners { get; set; }

    public virtual DbSet<OcopPartnerVerificationPaper> OcopPartnerVerificationPapers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderFee> OrderFees { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreShipper> StoreShippers { get; set; }

    public virtual DbSet<StoreStaff> StoreStaffs { get; set; }

    public virtual DbSet<SystemConfiguration> SystemConfigurations { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Batch");

            entity.HasIndex(e => e.DeliverBy, "deliver_by");

            entity.HasIndex(e => e.DeliveryZoneId, "delivery_zone_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompleteDate)
                .HasColumnType("datetime")
                .HasColumnName("complete_date");
            entity.Property(e => e.DeliverBy).HasColumnName("deliver_by");
            entity.Property(e => e.DeliveryStartDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_start_date");
            entity.Property(e => e.DeliveryZoneId).HasColumnName("delivery_zone_id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");

            entity.HasOne(d => d.DeliverByNavigation).WithMany(p => p.Batches)
                .HasForeignKey(d => d.DeliverBy)
                .HasConstraintName("Batch_ibfk_1");

            entity.HasOne(d => d.DeliveryZone).WithMany(p => p.Batches)
                .HasForeignKey(d => d.DeliveryZoneId)
                .HasConstraintName("Batch_ibfk_2");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Category");

            entity.HasIndex(e => e.OcopCategoryId, "ocop_category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.OcopCategoryId).HasColumnName("ocop_category_id");
            entity.Property(e => e.Type)
                .HasMaxLength(25)
                .HasColumnName("type")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.OcopCategory).WithMany(p => p.Categories)
                .HasForeignKey(d => d.OcopCategoryId)
                .HasConstraintName("Category_ibfk_1");
        });

        modelBuilder.Entity<DeliveryZone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Delivery_Zone");

            entity.HasIndex(e => e.ProvinceId, "province_id");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");

            entity.HasOne(d => d.Province).WithMany(p => p.DeliveryZones)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("Delivery_Zone_ibfk_1");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.RoleId, "role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .HasColumnName("first_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(25)
                .HasColumnName("last_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .HasColumnName("phone");
            entity.Property(e => e.PictureLink)
                .HasColumnType("text")
                .HasColumnName("picture_link");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Setting)
                .HasMaxLength(255)
                .HasColumnName("setting")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("Employee_ibfk_1");
        });

        modelBuilder.Entity<ExportFee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Export_Fee");

            entity.HasIndex(e => e.RequestId, "request_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdditionalFee)
                .HasPrecision(10, 2)
                .HasColumnName("additional_fee");
            entity.Property(e => e.DeliveryFee)
                .HasPrecision(10, 2)
                .HasColumnName("delivery_fee");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.RequestId).HasColumnName("request_id");

            entity.HasOne(d => d.Request).WithMany(p => p.ExportFees)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("Export_Fee_ibfk_1");
        });

        modelBuilder.Entity<Lot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Lot");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => e.RoomId, "room_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.ExportDate)
                .HasColumnType("datetime")
                .HasColumnName("export_date");
            entity.Property(e => e.ImportDate)
                .HasColumnType("datetime")
                .HasColumnName("import_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LotAmount).HasColumnName("lot_amount");
            entity.Property(e => e.LotNumber)
                .HasMaxLength(25)
                .HasColumnName("lot_number");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductPerLot).HasColumnName("product_per_lot");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.TotalProductAmount).HasColumnName("total_product_amount");

            entity.HasOne(d => d.Product).WithMany(p => p.Lots)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Lot_ibfk_2");

            entity.HasOne(d => d.Room).WithMany(p => p.Lots)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("Lot_ibfk_1");
        });

        modelBuilder.Entity<MoneyTransfer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Money_Transfer");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.HasIndex(e => e.TransferBy, "transfer_by");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(100)
                .HasColumnName("cancellation_reason")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ConfirmDate)
                .HasColumnType("datetime")
                .HasColumnName("confirm_date");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsTransferred)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_transferred");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.PictureLink)
                .HasColumnType("text")
                .HasColumnName("picture_link");
            entity.Property(e => e.TransferBy).HasColumnName("transfer_by");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.MoneyTransfers)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("Money_Transfer_ibfk_2");

            entity.HasOne(d => d.TransferByNavigation).WithMany(p => p.MoneyTransfers)
                .HasForeignKey(d => d.TransferBy)
                .HasConstraintName("Money_Transfer_ibfk_1");
        });

        modelBuilder.Entity<OcopCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("OCOP_Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Type)
                .HasMaxLength(25)
                .HasColumnName("type")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<OcopPartner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("OCOP_Partner");

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.HasIndex(e => e.OcopCategoryId, "ocop_category_id");

            entity.HasIndex(e => e.ProvinceId, "province_id");

            entity.HasIndex(e => e.RoleId, "role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .HasColumnName("bank_account_number");
            entity.Property(e => e.BankName)
                .HasMaxLength(25)
                .HasColumnName("bank_name");
            entity.Property(e => e.BusinessAddress)
                .HasMaxLength(100)
                .HasColumnName("business_address")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.BusinessName)
                .HasMaxLength(100)
                .HasColumnName("business_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.BusinessNameInternational)
                .HasMaxLength(100)
                .HasColumnName("business_name_international");
            entity.Property(e => e.BusinessShortName)
                .HasMaxLength(100)
                .HasColumnName("business_short_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .HasColumnName("first_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsVerified)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_verified");
            entity.Property(e => e.LastName)
                .HasMaxLength(25)
                .HasColumnName("last_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.OcopCategoryId).HasColumnName("ocop_category_id");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .HasColumnName("phone");
            entity.Property(e => e.PictureLink)
                .HasColumnType("text")
                .HasColumnName("picture_link");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Setting)
                .HasMaxLength(255)
                .HasColumnName("setting")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.TaxIdentificationNumber)
                .HasMaxLength(25)
                .HasColumnName("tax_identification_number");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Category).WithMany(p => p.OcopPartners)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("OCOP_Partner_ibfk_2");

            entity.HasOne(d => d.OcopCategory).WithMany(p => p.OcopPartners)
                .HasForeignKey(d => d.OcopCategoryId)
                .HasConstraintName("OCOP_Partner_ibfk_4");

            entity.HasOne(d => d.Province).WithMany(p => p.OcopPartners)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("OCOP_Partner_ibfk_1");

            entity.HasOne(d => d.Role).WithMany(p => p.OcopPartners)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("OCOP_Partner_ibfk_3");
        });

        modelBuilder.Entity<OcopPartnerVerificationPaper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("OCOP_Partner_Verification_Paper");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BackPictureLink)
                .HasColumnType("text")
                .HasColumnName("back_picture_link");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.FrontPictureLink)
                .HasColumnType("text")
                .HasColumnName("front_picture_link");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsRejected)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_rejected");
            entity.Property(e => e.IsVerified)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_verified");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.RejectDate)
                .HasColumnType("datetime")
                .HasColumnName("reject_date");
            entity.Property(e => e.RejectReason)
                .HasMaxLength(255)
                .HasColumnName("reject_reason")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.VerifyDate)
                .HasColumnType("datetime")
                .HasColumnName("verify_date");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.OcopPartnerVerificationPapers)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("OCOP_Partner_Verification_Paper_ibfk_1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Order");

            entity.HasIndex(e => e.BatchId, "batch_id");

            entity.HasIndex(e => e.DeliveryZoneId, "delivery_zone_id");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApproveDate)
                .HasColumnType("datetime")
                .HasColumnName("approve_date");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.CancelDate)
                .HasColumnType("datetime")
                .HasColumnName("cancel_date");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(100)
                .HasColumnName("cancellation_reason")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CompleteDate)
                .HasColumnType("datetime")
                .HasColumnName("complete_date");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.DeliverFinishDate)
                .HasColumnType("datetime")
                .HasColumnName("deliver_finish_date");
            entity.Property(e => e.DeliverStartDate)
                .HasColumnType("datetime")
                .HasColumnName("deliver_start_date");
            entity.Property(e => e.DeliveryZoneId).HasColumnName("delivery_zone_id");
            entity.Property(e => e.Distance)
                .HasPrecision(10, 2)
                .HasColumnName("distance");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.NumberOfTrips).HasColumnName("number_of_trips");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .HasColumnName("order_code");
            entity.Property(e => e.PictureLink)
                .HasColumnType("text")
                .HasColumnName("picture_link");
            entity.Property(e => e.ReceiverAddress)
                .HasMaxLength(100)
                .HasColumnName("receiver_address");
            entity.Property(e => e.ReceiverName)
                .HasMaxLength(50)
                .HasColumnName("receiver_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(11)
                .HasColumnName("receiver_phone");
            entity.Property(e => e.ReturnDate)
                .HasColumnType("datetime")
                .HasColumnName("return_date");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.TotalPriceAfterFee)
                .HasPrecision(10, 2)
                .HasColumnName("total_price_after_fee");
            entity.Property(e => e.TotalWeight)
                .HasPrecision(10, 2)
                .HasColumnName("total_weight");

            entity.HasOne(d => d.Batch).WithMany(p => p.Orders)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("Order_ibfk_3");

            entity.HasOne(d => d.DeliveryZone).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DeliveryZoneId)
                .HasConstraintName("Order_ibfk_1");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("Order_ibfk_2");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Order_Detail");

            entity.HasIndex(e => e.LotId, "lot_id");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LotId).HasColumnName("lot_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductAmount).HasColumnName("product_amount");
            entity.Property(e => e.ProductPrice)
                .HasPrecision(10, 2)
                .HasColumnName("product_price");

            entity.HasOne(d => d.Lot).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.LotId)
                .HasConstraintName("Order_Detail_ibfk_2");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("Order_Detail_ibfk_1");
        });

        modelBuilder.Entity<OrderFee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Order_Fee");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdditionalFee)
                .HasPrecision(10, 2)
                .HasColumnName("additional_fee");
            entity.Property(e => e.DeliveryFee)
                .HasPrecision(10, 2)
                .HasColumnName("delivery_fee");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.StorageFee)
                .HasPrecision(10, 2)
                .HasColumnName("storage_fee");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderFees)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("Order_Fee_ibfk_1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.CollectedBy, "collected_by");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CollectedBy).HasColumnName("collected_by");
            entity.Property(e => e.IsConfirmed)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_confirmed");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.CollectedByNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CollectedBy)
                .HasConstraintName("Payment_ibfk_2");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("Payment_ibfk_1");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("Payment_ibfk_3");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Product");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.HasIndex(e => e.ProductCategoryId, "product_category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Barcode)
                .HasMaxLength(255)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsCold)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_cold");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.Origin)
                .HasMaxLength(25)
                .HasColumnName("origin")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PictureLink)
                .HasColumnType("text")
                .HasColumnName("picture_link");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductCategoryId).HasColumnName("product_category_id");
            entity.Property(e => e.Unit)
                .HasMaxLength(25)
                .HasColumnName("unit")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.Products)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("Product_ibfk_1");

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategoryId)
                .HasConstraintName("Product_ibfk_2");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Product_Category");

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
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

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("Product_Category_ibfk_1");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Province");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.SubDivisionName)
                .HasMaxLength(25)
                .HasColumnName("sub_division_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Request");

            entity.HasIndex(e => e.ExportFromLotId, "export_from_lot_id");

            entity.HasIndex(e => e.LotId, "lot_id");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.HasIndex(e => e.SendToRoomId, "send_to_room_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApporveDate)
                .HasColumnType("datetime")
                .HasColumnName("apporve_date");
            entity.Property(e => e.CancelDate)
                .HasColumnType("datetime")
                .HasColumnName("cancel_date");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(100)
                .HasColumnName("cancellation_reason")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.DeliverDate)
                .HasColumnType("datetime")
                .HasColumnName("deliver_date");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ExportFromLotId).HasColumnName("export_from_lot_id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LotId).HasColumnName("lot_id");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.RequestType)
                .HasMaxLength(10)
                .HasColumnName("request_type");
            entity.Property(e => e.SendToRoomId).HasColumnName("send_to_room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");

            entity.HasOne(d => d.ExportFromLot).WithMany(p => p.RequestExportFromLots)
                .HasForeignKey(d => d.ExportFromLotId)
                .HasConstraintName("Request_ibfk_2");

            entity.HasOne(d => d.Lot).WithMany(p => p.RequestLots)
                .HasForeignKey(d => d.LotId)
                .HasConstraintName("Request_ibfk_1");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.Requests)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("Request_ibfk_4");

            entity.HasOne(d => d.SendToRoom).WithMany(p => p.Requests)
                .HasForeignKey(d => d.SendToRoomId)
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
                .HasMaxLength(10)
                .HasColumnName("role_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Room");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.HasIndex(e => e.StoreId, "store_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BoughtDate)
                .HasColumnType("datetime")
                .HasColumnName("bought_date");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.IsCold)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_cold");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.MaxWeight)
                .HasPrecision(10, 2)
                .HasColumnName("max_weight");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Width)
                .HasPrecision(10, 2)
                .HasColumnName("width");
            entity.Property(e => e.Length)
                .HasPrecision(10, 2)
                .HasColumnName("length");
            entity.Property(e => e.X)
                .HasPrecision(10, 2)
                .HasColumnName("x");
            entity.Property(e => e.Y)
                .HasPrecision(10, 2)
                .HasColumnName("y");
            entity.Property(e => e.RoomCode)
                .HasMaxLength(25)
                .HasColumnName("room_code");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("Room_ibfk_2");

            entity.HasOne(d => d.Store).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("Room_ibfk_1");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Store");

            entity.HasIndex(e => e.ProvinceId, "province_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity)
                .HasPrecision(10, 2)
                .HasColumnName("capacity");
            entity.Property(e => e.Longitude)
                .HasPrecision(11, 7)
                .HasColumnName("longitude");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 7)
                .HasColumnName("latitude");
            entity.Property(e => e.Width)
                .HasPrecision(10, 2)
                .HasColumnName("width");
            entity.Property(e => e.Length)
                .HasPrecision(10, 2)
                .HasColumnName("length");
            entity.Property(e => e.Cols)
                .HasColumnName("cols")
                .HasColumnType("int");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");

            entity.HasOne(d => d.Province).WithMany(p => p.Stores)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("Store_ibfk_1");
        });

        modelBuilder.Entity<StoreShipper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Store_Shipper");

            entity.HasIndex(e => e.DeliveryZoneId, "delivery_zone_id");

            entity.HasIndex(e => e.EmployeeId, "employee_id");

            entity.HasIndex(e => e.StoreId, "store_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeliveryZoneId).HasColumnName("delivery_zone_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            entity.HasOne(d => d.DeliveryZone).WithMany(p => p.StoreShippers)
                .HasForeignKey(d => d.DeliveryZoneId)
                .HasConstraintName("Store_Shipper_ibfk_2");

            entity.HasOne(d => d.Employee).WithMany(p => p.StoreShippers)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("Store_Shipper_ibfk_1");

            entity.HasOne(d => d.Store).WithMany(p => p.StoreShippers)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("Store_Shipper_ibfk_3");
        });

        modelBuilder.Entity<StoreStaff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Store_Staff");

            entity.HasIndex(e => e.EmployeeId, "employee_id");

            entity.HasIndex(e => e.StoreId, "store_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.StoreStaffs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("Store_Staff_ibfk_2");

            entity.HasOne(d => d.Store).WithMany(p => p.StoreStaffs)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("Store_Staff_ibfk_1");
        });

        modelBuilder.Entity<SystemConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("SystemConfiguration");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdditonalUpdateFeeRatePercentage)
                .HasPrecision(10, 2)
                .HasColumnName("additonal_update_fee_rate_percentage");
            entity.Property(e => e.DeliveryDistanceFeeRate)
                .HasPrecision(10, 2)
                .HasColumnName("delivery_distance_fee_rate");
            entity.Property(e => e.DeliveryDistanceTreshold)
                .HasPrecision(10, 2)
                .HasColumnName("delivery_distance_treshold");
            entity.Property(e => e.DeliveryDistanceUnit)
                .HasPrecision(10, 2)
                .HasColumnName("delivery_distance_unit");
            entity.Property(e => e.DeliveryWeightFeeRate)
                .HasPrecision(10, 2)
                .HasColumnName("delivery_weight_fee_rate");
            entity.Property(e => e.DeliveryWeightUnit)
                .HasPrecision(10, 2)
                .HasColumnName("delivery_weight_unit");
            entity.Property(e => e.DeliveryWeigtTreshold)
                .HasPrecision(10, 2)
                .HasColumnName("delivery_weigt_treshold");
            entity.Property(e => e.ExportDeliveryDistanceFeeRate)
                .HasPrecision(10, 2)
                .HasColumnName("export_delivery_distance_fee_rate");
            entity.Property(e => e.ExportDeliveryDistanceTreshold)
                .HasPrecision(10, 2)
                .HasColumnName("export_delivery_distance_treshold");
            entity.Property(e => e.ExportDeliveryDistanceUnit)
                .HasPrecision(10, 2)
                .HasColumnName("export_delivery_distance_unit");
            entity.Property(e => e.ExportDeliveryWeightFeeRate)
                .HasPrecision(10, 2)
                .HasColumnName("export_delivery_weight_fee_rate");
            entity.Property(e => e.ExportDeliveryWeightTreshold)
                .HasPrecision(10, 2)
                .HasColumnName("export_delivery_weight_treshold");
            entity.Property(e => e.ExportDeliveryWeightUnit)
                .HasPrecision(10, 2)
                .HasColumnName("export_delivery_weight_unit");
            entity.Property(e => e.InventoryExpireEmailNotificationPeriod).HasColumnName("inventory_expire_email_notification_period");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.OrderDistanceLimit)
                .HasPrecision(10, 2)
                .HasColumnName("order_distance_limit");
            entity.Property(e => e.OrderWeightLimit)
                .HasPrecision(10, 2)
                .HasColumnName("order_weight_limit");
            entity.Property(e => e.RequestDistanceLimit)
                .HasPrecision(10, 2)
                .HasColumnName("request_distance_limit");
            entity.Property(e => e.RequestWeightLimit)
                .HasPrecision(10, 2)
                .HasColumnName("request_weight_limit");
            entity.Property(e => e.StorageFeeFirstPeriodDuration)
                .HasPrecision(10, 2)
                .HasColumnName("storage_fee_first_period_duration");
            entity.Property(e => e.StorageFeeFirstPeriodRate)
                .HasPrecision(10, 2)
                .HasColumnName("storage_fee_first_period_rate");
            entity.Property(e => e.StorageFeePersonalFeePerLot)
                .HasPrecision(10, 2)
                .HasColumnName("storage_fee_personal_fee_per_lot");
            entity.Property(e => e.StorageFeeSecondPeriodDuration)
                .HasPrecision(10, 2)
                .HasColumnName("storage_fee_second_period_duration");
            entity.Property(e => e.StorageFeeSecondPeriodRate)
                .HasPrecision(10, 2)
                .HasColumnName("storage_fee_second_period_rate");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Transaction");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.HasIndex(e => e.RoomId, "room_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(100)
                .HasColumnName("cancellation_reason")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.MonthAmount).HasColumnName("month_amount");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("Transaction_ibfk_1");

            entity.HasOne(d => d.Room).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("Transaction_ibfk_2");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Unit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Vehicle");

            entity.HasIndex(e => e.AssignedDriverId, "assigned_driver_id");

            entity.HasIndex(e => e.StoreId, "store_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedDriverId).HasColumnName("assigned_driver_id");
            entity.Property(e => e.Capacity)
                .HasPrecision(10, 2)
                .HasColumnName("capacity");
            entity.Property(e => e.IsCold)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_cold");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(25)
                .HasColumnName("license_plate");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");

            entity.HasOne(d => d.AssignedDriver).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.AssignedDriverId)
                .HasConstraintName("Vehicle_ibfk_1");

            entity.HasOne(d => d.Store).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("Vehicle_ibfk_2");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Wallet");

            entity.HasIndex(e => e.OcopPartnerId, "ocop_partner_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_deleted");
            entity.Property(e => e.OcopPartnerId).HasColumnName("ocop_partner_id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.OcopPartner).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.OcopPartnerId)
                .HasConstraintName("Wallet_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}





