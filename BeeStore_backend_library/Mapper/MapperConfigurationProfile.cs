﻿using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.Batch;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.PaymentDTOs;
using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.DTO.ProvinceDTOs;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.DTO.RoleDTOs;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.DTO.VehicleDTOs;
using BeeStore_Repository.DTO.WalletDTO;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Mapper.CustomResolver;
using BeeStore_Repository.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace BeeStore_Repository.Mapper
{

    public class MapperConfigurationsProfile : Profile
    {
        public MapperConfigurationsProfile()
        {
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));
            CreateMap(typeof(Task<>), typeof(Pagination<>));
            CreateMap(typeof(InternalDbSet<>), typeof(IQueryable<>));

            CreateMap<EmployeeCreateRequest, Employee>();
            CreateMap<UserSignUpRequestDTO, OcopPartner>();

            CreateMap<Province, ProvinceListDTO>();

            CreateMap<Employee, EmployeeListDTO>()
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
                    .ForMember(dest => dest.WorkAtWarehouseId, opt => opt.MapFrom((src, dest) =>
                                src.WarehouseShippers.FirstOrDefault(u => u.EmployeeId.Equals(src.Id)
                                                                       && u.IsDeleted == false)?.WarehouseId ??
                                src.WarehouseStaffs.FirstOrDefault(u => u.EmployeeId.Equals(src.Id)
                                                                     && u.IsDeleted == false)?.WarehouseId))
                    .ForMember(dest => dest.WorkAtWarehouseName, opt => opt.MapFrom((src, dest) =>
                                src.WarehouseShippers.FirstOrDefault(u => u.EmployeeId.Equals(src.Id)
                                                                       && u.IsDeleted == false)?.Warehouse.Name ??
                                src.WarehouseStaffs.FirstOrDefault(u => u.EmployeeId.Equals(src.Id)
                                                                     && u.IsDeleted == false)?.Warehouse.Name))
                    .ForMember(dest => dest.WarehouseLocation, opt => opt.MapFrom((src, dest) =>
                                src.WarehouseShippers.FirstOrDefault(u => u.EmployeeId.Equals(src.Id)
                                                                       && u.IsDeleted == false)?.Warehouse.Location ??
                                src.WarehouseStaffs.FirstOrDefault(u => u.EmployeeId.Equals(src.Id)
                                                                     && u.IsDeleted == false)?.Warehouse.Location))
                    .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Role, RoleListDTO>();

            CreateMap<Wallet, WalletDTO>()
                .ForMember(dest => dest.partner_email, opt => opt.MapFrom(src => src.OcopPartner.Email))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Transaction, TransactionListDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.OcopPartner.Email))
                .ForMember(dest => dest.InventoryName, opt => opt.MapFrom(src => src.Inventory.Name))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<OcopPartner, PartnerListDTO>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
                .ForMember(dest => dest.ProvinceCode, opt => opt.MapFrom(src => src.Province.Code))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Type))
                .ForMember(dest => dest.OcopCategoryName, opt => opt.MapFrom(src => src.OcopCategory.Type))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Warehouse, WarehouseListDTO>()
                .ForMember(dest => dest.UnboughtInventory, opt => opt.MapFrom(src => src.Inventories.Count(u => !u.OcopPartnerId.HasValue)))
                .ForMember(dest => dest.TotalInventory, opt => opt.MapFrom(src => src.Inventories.Count()))
                .ForMember(dest => dest.AvailableCapacity, opt => opt.MapFrom(src => src.Inventories == null ? 0 : src.Inventories.Where(u => string.IsNullOrEmpty(u.OcopPartnerId.ToString()) || u.OcopPartnerId == null).Sum(u => u.MaxWeight)))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location + ", " + src.Province.SubDivisionName))
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Province.SubDivisionName));
            CreateMap<WarehouseCreateDTO, Warehouse>();
            CreateMap<DeliveryZoneCreateDTO, DeliveryZone>();
            CreateMap<Warehouse, WarehouseListInventoryDTO>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Province.SubDivisionName))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location + ", " + src.Province.SubDivisionName))
                .ForMember(dest => dest.TotalInventory, opt => opt.MapFrom(src => src.Inventories.Count()))
                .ForMember(dest => dest.Inventories, opt => opt.MapFrom(src => src.Inventories))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Warehouse, WarehouseDeliveryZoneDTO>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location + ", " + src.Province.SubDivisionName))
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Province.SubDivisionName))
                .ForMember(dest => dest.DeliveryZones, opt => opt.MapFrom(src => src.Province.DeliveryZones.Where(u => u.ProvinceId.Equals(src.ProvinceId))));

            CreateMap<DeliveryZone, DeliveryZoneDTO>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Province.SubDivisionName));

            CreateMap<Inventory, InventoryListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.WarehouseLocation, opt => opt.MapFrom(src => src.Warehouse.Location + ", " + src.Warehouse.Province.SubDivisionName))
                .ForMember(dest => dest.IsCold, opt => opt.MapFrom(src => src.Warehouse!.IsCold))
                .ForMember(dest => dest.totalProduct, opt => opt.MapFrom<CustomTotalProductInventoryResolver>())
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<InventoryCreateDTO, Inventory>();
            CreateMap<Inventory, InventoryLotListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.Lots, opt => opt.MapFrom(src => src.Lots))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Product, ProductListDTO>()
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory!.TypeName))
                .ForMember(dest => dest.IsInInv, opt => opt.MapFrom(src => src.Lots.Any(u => u.InventoryId.HasValue && u.IsDeleted == false)))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ProductCreateDTO, Product>();

            CreateMap<Category, CategoryListDTO>();
            CreateMap<OcopCategory, OcopCategoryListDTO>();
            CreateMap<ProductCategory, ProductCategoryListDTO>();
            CreateMap<ProductCategoryCreateDTO, ProductCategory>();


            CreateMap<Lot, LotListDTO>()
                .ForMember(dest => dest.ProductUnit, opt => opt.MapFrom(src => src.Product.Unit))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPictureLink, opt => opt.MapFrom(src => src.Product.PictureLink))
                .ForMember(dest => dest.isCold, opt => opt.MapFrom(src => src.Product.IsCold))
                .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => src.Inventory.WarehouseId))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Inventory.Warehouse.Name))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<LotCreateDTO, Lot>();

            CreateMap<Vehicle, VehicleListDTO>()
                .ForMember(dest => dest.AssignedDriverName, opt => opt.MapFrom(src => src.AssignedDriver.FirstName + " " + src.AssignedDriver.LastName))
                .ForMember(dest => dest.AssignedDriverEmail, opt => opt.MapFrom(src => src.AssignedDriver.Email))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<VehicleCreateDTO, Vehicle>();



            CreateMap<WarehouseShipper, WarehouseShipperListDTO>()
                .ForMember(dest => dest.ShipperName, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.LastName}"))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Employee!.Email))
                .ForMember(dest => dest.DeliveryZoneName, opt => opt.MapFrom(src => src.DeliveryZone.Name))
                .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src => src.Employee.Vehicles))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<WarehouseShipperCreateDTO, WarehouseShipper>();


            CreateMap<WarehouseStaff, WarehouseStaffListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Employee!.Email))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<WarehouseStaffCreateDTO, WarehouseStaff>();

            CreateMap<Request, RequestListDTO>()
                .ForMember(dest => dest.LotAmount, opt => opt.MapFrom(src => src.Lot.LotAmount))
                .ForMember(dest => dest.ProductPerLotAmount, opt => opt.MapFrom(src => src.Lot.ProductPerLot))
                .ForMember(dest => dest.TotalProductAmount, opt => opt.MapFrom(src => src.Lot.TotalProductAmount))
                .ForMember(dest => dest.partner_email, opt => opt.MapFrom(src => src.OcopPartner!.Email))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Lot!.Product!.Name))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.SendToInventory!.Warehouse!.Name))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Lot!.Product!.PictureLink))
                .ForMember(dest => dest.ExportFromInventoryName, opt => opt.MapFrom(src => src.ExportFromLot.Inventory.Name))
                .ForMember(dest => dest.SendToInventoryName, opt => opt.MapFrom(src => src.SendToInventory.Name))
                .ForMember(dest => dest.ExportFromWarehouseName, opt => opt.MapFrom(src => src.ExportFromLot.Inventory.Warehouse.Name))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<RequestCreateDTO, Request>();

            CreateMap<Order, OrderListDTO>()
                .ForMember(dest => dest.partner_email, opt => opt.MapFrom(src => src.OcopPartner!.Email))
                .ForMember(dest => dest.WarehouseID, opt => opt.MapFrom(src => src.OrderDetails.FirstOrDefault(u => u.OrderId.Equals(src.Id)).Lot.Inventory.WarehouseId))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.OrderDetails.FirstOrDefault(u => u.OrderId.Equals(src.Id)).Lot.Inventory.Warehouse.Name))
                .ForMember(dest => dest.WarehouseLocation, opt => opt.MapFrom(src => src.OrderDetails.FirstOrDefault(u => u.OrderId.Equals(src.Id)).Lot.Inventory.Warehouse.Location))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.OrderFees, opt => opt.MapFrom(src => src.OrderFees))
                .ForMember(dest => dest.DeliveryZoneName, opt => opt.MapFrom(src => src.DeliveryZone.Name))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<OrderDetail, OrderDetailDTO>()
                .ForMember(dest => dest.LotName, opt => opt.MapFrom(src => src.Lot.Name))
                .ForMember(dest => dest.InventoryId, opt => opt.MapFrom(src => src.Lot.InventoryId))
                .ForMember(dest => dest.InventoryName, opt => opt.MapFrom(src => src.Lot.Inventory.Name))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Lot.ProductId))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Lot != null ? src.Lot.Product.PictureLink : null))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Lot.Product.Name))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Lot.Product.Unit))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Lot.Product.Weight));
            CreateMap<OrderFee, OrderFeeDTO>();
            CreateMap<OrderCreateDTO, Order>();
            CreateMap<OrderUpdateDTO, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<OrderDetailCreateDTO, OrderDetail>();
            CreateMap<Payment, PaymentListDTO>()
                .ForMember(dest => dest.ShipperEmail, opt => opt.MapFrom(src => src.CollectedByNavigation.Email))
                .ForMember(dest => dest.ShipperName, opt => opt.MapFrom(src => src.CollectedByNavigation.FirstName + src.CollectedByNavigation.LastName));
            CreateMap<OrderUpdateDTO, List<OrderDetail>>()
           .ConvertUsing((src, dest) => src.OrderDetails.Select(od => new OrderDetail
           {
               LotId = od.LotId,
               ProductPrice = od.ProductPrice,
               ProductAmount = od.ProductAmount
           }).ToList());
            CreateMap<Batch, BatchListDTO>()
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders))
                .ForMember(dest => dest.ShipperEmail, opt => opt.MapFrom(src => src.DeliverByNavigation.Email))
                .ForMember(dest => dest.ShipperName, opt => opt.MapFrom(src => $"{src.DeliverByNavigation.FirstName} {src.DeliverByNavigation.LastName}"))
                .ForMember(dest => dest.DeliveryZoneName, opt => opt.MapFrom(src => src.DeliveryZone.Name));
            CreateMap<BatchCreateDTO, Batch>()
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<MoneyTransfer, MoneyTransferListDTO>()
                .ForMember(dest => dest.partner_email, opt => opt.MapFrom(src => src.OcopPartner.Email))
                .ForMember(dest => dest.partner_bank_name, opt => opt.MapFrom(src => src.OcopPartner.BankName))
                .ForMember(dest => dest.partner_bank_account, opt => opt.MapFrom(src => src.OcopPartner.BankAccountNumber))
                .ForMember(dest => dest.TransferByStaffEmail, opt => opt.MapFrom(src => src.TransferByNavigation.Email))
                .ForMember(dest => dest.TransferByStaffName, opt => opt.MapFrom(src => $"{src.TransferByNavigation.FirstName} {src.TransferByNavigation.LastName}"))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
