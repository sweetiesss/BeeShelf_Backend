﻿using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using Mysqlx;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Pagination<UserListDTO>> GetAllUser(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.UserRepo.GetAllAsync();
            var result = _mapper.Map<List<UserListDTO>>(list);
            
            return await ListPagination<UserListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<UserListDTO> Login(string email, string password)
        {
            var user = await _unitOfWork.UserRepo.GetQueryable();
            var exist = await user.Where(a => a.Email == email).FirstOrDefaultAsync();
            if (exist != null)
            {
                if (exist.Password.Equals(password))
                {
                    return _mapper.Map<UserListDTO>(exist);
                }
                else
                {
                    throw new Exception("Password is incorrect");
                    ///PLACEHOLDER WILL CHANGE LATER
                }
            }
            else
            {
                throw new Exception("Email and password is incorrect");
            }
        }
    }
}
