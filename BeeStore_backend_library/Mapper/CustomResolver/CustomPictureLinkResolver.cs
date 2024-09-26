﻿using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomPictureLinkResolver : IValueResolver<User, UserListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomPictureLinkResolver(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(User source, UserListDTO destination, string destMember, ResolutionContext context)
        {
            var picture = _context.Pictures.FirstOrDefault(r => r.Id == source.PictureId);
            return picture != null ? picture.PictureLink : null;
        }
    }
}
