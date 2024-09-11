using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
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


    }
}
