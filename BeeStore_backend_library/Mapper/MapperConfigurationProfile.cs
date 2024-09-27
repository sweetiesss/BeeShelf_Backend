﻿using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.DTO.WarehouseDTOs;
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
                    .ForMember(dest => dest.Picture_Link, opt => opt.MapFrom<CustomPictureLinkResolver>());
            CreateMap<Partner, PartnerListDTO>();
            CreateMap<UpgradeToPartnerRequest, Partner>();
            CreateMap<Warehouse, WarehouseListDTO>();
            CreateMap<WarehouseCreateDTO, Warehouse>();
            }
        }
    }
