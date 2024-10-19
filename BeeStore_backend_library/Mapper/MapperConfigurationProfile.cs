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
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
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
            CreateMap(typeof(UserCreateRequestDTO), typeof(User));

            CreateMap<UserCreateRequestDTO, User>()
                 .ForMember(dest => dest.RoleId, opt => opt.MapFrom<CustomRoleNameResolver<UserCreateRequestDTO>>())
                     .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));
            CreateMap<UserSignUpRequestDTO, User>()
                 .ForMember(dest => dest.RoleId, opt => opt.MapFrom<CustomRoleNameResolver<UserSignUpRequestDTO>>());
            CreateMap<User, UserListDTO>()
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
                    .ForMember(dest => dest.Picture_Link, opt => opt.MapFrom(src => src.Picture!.PictureLink))
                    .ForMember(dest => dest.BankAccountNumber, opt => opt.MapFrom(src => src.Partners
                                                                                            .FirstOrDefault(u => u.UserId.Equals(src.Id))!
                                                                                            .BankAccountNumber))
                    .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.Partners
                                                                                            .FirstOrDefault(u => u.UserId.Equals(src.Id))!
                                                                                            .BankName))
                    .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Role, RoleListDTO>();

            CreateMap<Partner, PartnerListDTO>()
                .ForMember(dest => dest.User_Email, opt => opt.MapFrom(src => src.User!.Email))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<PartnerUpdateRequest, Partner>();


            CreateMap<Warehouse, WarehouseListDTO>();
            CreateMap<WarehouseCreateDTO, Warehouse>();
            CreateMap<Warehouse, WarehouseListInventoryDTO>()
                .ForMember(dest => dest.TotalInventory, opt => opt.MapFrom(src => src.Inventories.Count()))
                .ForMember(dest => dest.Inventories, opt => opt.MapFrom(src => src.Inventories))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Inventory, InventoryListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<InventoryCreateDTO, Inventory>();
            CreateMap<Inventory, InventoryListPackagesDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.Packages))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Product, ProductListDTO>()
                .ForMember(dest => dest.Picture_Link, opt => opt.MapFrom(src => src.Picture!.PictureLink))
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory!.TypeName))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ProductCreateDTO, Product>();


            CreateMap<ProductCategory, ProductCategoryListDTO>();
            CreateMap<ProductCategoryCreateDTO, ProductCategory>();


            CreateMap<Package, PackageListDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<PackageCreateDTO, Package>();


            CreateMap<WarehouseCategory, WarehouseCategoryListDTO>()
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory!.TypeName))
                .ForMember(dest => dest.warehouse_name, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<WarehouseCategoryCreateDTO, WarehouseCategory>();


            CreateMap<WarehouseShipper, WarehouseShipperListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.user_email, opt => opt.MapFrom(src => src.User!.Email))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<WarehouseShipperCreateDTO, WarehouseShipper>();


            CreateMap<WarehouseStaff, WarehouseStaffListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.Name))
                .ForMember(dest => dest.user_email, opt => opt.MapFrom(src => src.User!.Email))
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<WarehouseStaffCreateDTO, WarehouseStaff>();

            CreateMap<Request, RequestListDTO>()
                .ForMember(dest => dest.user_email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Package!.Product!.Name))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<RequestCreateDTO, Request>();

            CreateMap<Order, OrderListDTO>()
                .ForMember(dest => dest.user_email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.deliver_by, opt => opt.MapFrom(src => src.DeliverByNavigation!.Email))
                .ForMember(dest => dest.picture_link, opt => opt.MapFrom(src => src.Picture!.PictureLink))
                .ForMember(dest => dest.product_name, opt => opt.MapFrom(src => src.Product!.Name))
            .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<OrderCreateDTO, Order>();
        }
    }
}
