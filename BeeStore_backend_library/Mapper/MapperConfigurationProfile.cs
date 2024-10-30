using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.DTO.RoleDTOs;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.DTO.WalletDTO;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
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
            CreateMap(typeof(EmployeeCreateRequest), typeof(User));

            CreateMap<EmployeeCreateRequest, Employee>()
                     .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));
            CreateMap<UserSignUpRequestDTO, OcopPartner>();
            CreateMap<Employee, EmployeeListDTO>()
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
                    .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Role, RoleListDTO>();

            CreateMap<Wallet, WalletDTO>()
                .ForMember(dest => dest.partner_email, opt => opt.MapFrom(src => src.OcopPartner.Email))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<OcopPartner, PartnerListDTO>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
                .ForMember(dest => dest.ProvinceCode, opt => opt.MapFrom(src => src.Province.Code))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Type))
                .ForMember(dest => dest.OcopCategoryName, opt => opt.MapFrom(src => src.OcopCategory.Type))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Warehouse, WarehouseListDTO>();
            CreateMap<WarehouseCreateDTO, Warehouse>();
            CreateMap<Warehouse, WarehouseListInventoryDTO>()
                .ForMember(dest => dest.TotalInventory, opt => opt.MapFrom(src => src.Inventories.Count()))
                .ForMember(dest => dest.Inventories, opt => opt.MapFrom(src => src.Inventories))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Warehouse, WarehouseDeliveryZoneDTO>()
                .ForMember(dest => dest.DeliveryZones, opt => opt.MapFrom(src => src.DeliveryZones));

            CreateMap<DeliveryZone, DeliveryZoneDTO>();

            CreateMap<Inventory, InventoryListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<InventoryCreateDTO, Inventory>();
            CreateMap<Inventory, InventoryLotListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.Lots, opt => opt.MapFrom(src => src.Lots))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Product, ProductListDTO>()
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory!.TypeName))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ProductCreateDTO, Product>();


            CreateMap<ProductCategory, ProductCategoryListDTO>();
            CreateMap<ProductCategoryCreateDTO, ProductCategory>();


            CreateMap<Lot, LotListDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<LotCreateDTO, Lot>();


            CreateMap<WarehouseShipper, WarehouseShipperListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Employee!.Email))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<WarehouseShipperCreateDTO, WarehouseShipper>();


            CreateMap<WarehouseStaff, WarehouseStaffListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Employee!.Email))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<WarehouseStaffCreateDTO, WarehouseStaff>();

            CreateMap<Request, RequestListDTO>()
                .ForMember(dest => dest.partner_email, opt => opt.MapFrom(src => src.OcopPartner!.Email))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Lot!.Product!.Name))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<RequestCreateDTO, Request>();

            CreateMap<Order, OrderListDTO>()
                .ForMember(dest => dest.partner_email, opt => opt.MapFrom(src => src.OcopPartner!.Email))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<OrderDetail, OrderDetailDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Lot.Product.Name));
            CreateMap<OrderCreateDTO, Order>();
            CreateMap<OrderDetailCreateDTO, OrderDetail>();
        }
    }
}
