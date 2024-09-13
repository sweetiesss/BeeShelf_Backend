using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly ILoggerManager _logger;
        public UserService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Pagination<UserListDTO>> GetAllUser(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.UserRepo.GetAllAsync();
            if(list == null)
            {
                _logger.LogError("No user found.");
            }

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
                        _logger.LogError("Password is incorrect user id: " + exist.Id);
                    throw new KeyNotFoundException("Password is incorrect");
                        ///PLACEHOLDER WILL CHANGE LATER
                    }
                }
                else
                {
                throw new KeyNotFoundException("User not found");   
                }
            }
    }
}
