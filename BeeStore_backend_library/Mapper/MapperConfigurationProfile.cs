using AutoMapper;
using BeeStore_Repository.DTO;
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
            //CreateMap<UserListDTO, User>();
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));
            CreateMap(typeof(Task<>), typeof(Pagination<>));
            CreateMap(typeof(InternalDbSet<>), typeof(IQueryable<>));

            //CreateMap<User, UserListDTO>();
            CreateMap<UserListDTO, User>();
                CreateMap<User, UserListDTO>()
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom<CustomRoleNameResolver>());
                
            }
        }
    }
