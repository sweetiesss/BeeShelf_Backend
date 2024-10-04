using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Mapper.CustomResolver;
using BeeStore_Repository.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                 .ForMember(dest => dest.RoleId, opt => opt.MapFrom<CustomRoleNameReverseResolver>());
            CreateMap<User, UserListDTO>()
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom<CustomRoleNameResolver>())
                    .ForMember(dest => dest.Picture_Link, opt => opt.MapFrom<CustomPictureLinkResolverUser>());


            CreateMap<Partner, PartnerListDTO>()
                .ForMember(dest => dest.User_Email, opt => opt.MapFrom<CustomUserEmailResolverPartner>());
            CreateMap<PartnerUpdateRequest, Partner>();


            CreateMap<Warehouse, WarehouseListDTO>();
            CreateMap<WarehouseCreateDTO, Warehouse>();


            CreateMap<Inventory, InventoryListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom<CustomWarehouseNameResolver>());
            CreateMap<InventoryCreateDTO, Inventory>();


            CreateMap<Product, ProductListDTO>()
                .ForMember(dest => dest.Picture_Link, opt => opt.MapFrom<CustomPictureLinkResolverProduct>())
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom<CustomProductCategoryResolverProduct>());
            CreateMap<ProductCreateDTO, Product>();


            CreateMap<ProductCategory, ProductCategoryListDTO>();
            CreateMap<ProductCategoryCreateDTO, ProductCategory>();


            CreateMap<Package, PackageListDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom<CustomProductNameResolverPackage>());
            CreateMap<PackageCreateDTO, Package>();


            CreateMap<WarehouseCategory, WarehouseCategoryListDTO>()
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom<CustomProductCategoryResolverWarehouseCategory>())
                .ForMember(dest => dest.warehouse_name, opt => opt.MapFrom<CustomWarehouseNameResolverWarehouseCategory>());
            CreateMap<WarehouseCategoryCreateDTO, WarehouseCategory>();


            CreateMap<WarehouseShipper, WarehouseShipperListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom<CustomWarehouseNameResolverWarehouseShipper>())
                .ForMember(dest => dest.user_email, opt => opt.MapFrom<CustomUserEmailResolverWarehouseShipper>());
            CreateMap<WarehouseShipperCreateDTO,  WarehouseShipper>();


            CreateMap<WarehouseStaff, WarehouseStaffListDTO>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom<CustomWarehouseNameResolverWarehouseStaff>())
                .ForMember(dest => dest.user_email, opt => opt.MapFrom<CustomUserEmailResolverWarehouseStaff>());
            CreateMap<WarehouseStaffCreateDTO, WarehouseStaff>();

            CreateMap<Request, RequestListDTO>()
                .ForMember(dest => dest.user_email, opt => opt.MapFrom<CustomUserEmailResolverRequest>())
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom<CustomProductNameResolverRequest>());
            CreateMap<RequestCreateDTO, Request>();

            CreateMap<Order, OrderListDTO>()
                .ForMember(dest => dest.user_email, opt => opt.MapFrom<CustomUserEmailResolverOrder>())
                .ForMember(dest => dest.deliver_by, opt => opt.MapFrom<CustomUserEmailShipperResolverOrder>())
                .ForMember(dest => dest.picture_link, opt => opt.MapFrom<CustomPictureLinkResolverOrder>())
                .ForMember(dest => dest.product_name, opt => opt.MapFrom<CustomProductNameResolverOrder>());
            CreateMap<OrderCreateDTO, Order>();
        }
        }
    }
    