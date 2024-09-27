using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public async Task<UserCreateRequestDTO> CreateUser(UserCreateRequestDTO user)
        {
            Expression<Func<User, bool>> keySelector = u => u.Email == user.Email;
            var exist = await _unitOfWork.UserRepo.GetByKeyAsync(user.Email, keySelector);
            if (exist != null)
            {
                throw new DuplicateException("Email already exist");
            }
            var result = _mapper.Map<User>(user);
            await _unitOfWork.UserRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return user;
        }

        public async Task<string> DeleteUser(string email)
        {
            Expression<Func<User, bool>> keySelector = u => u.Email == email;
            var exist = await _unitOfWork.UserRepo.GetByKeyAsync(email, keySelector);
            if (exist != null)
            {
                _unitOfWork.UserRepo.SoftDelete(exist);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new KeyNotFoundException("User not found");
            }
            return "Success";
        }

        public async Task<Pagination<UserListDTO>> GetAllUser(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.UserRepo.GetAllAsync();
            if (list == null)
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

        public async Task<UserUpdateRequestDTO> UpdateUser(UserUpdateRequestDTO user)
        {
            Expression<Func<User, bool>> keySelector = u => u.Email == user.Email;
            var exist = await _unitOfWork.UserRepo.GetByKeyAsync(user.Email, keySelector);
            if (exist != null)
            {
                if (!String.IsNullOrEmpty(user.Password) && !user.Password.Equals("string"))
                {
                    exist.Password = user.Password;
                }
                if (!user.PictureId.Equals(0))
                {
                    exist.PictureId = user.PictureId;
                }
                if (!String.IsNullOrEmpty(user.Phone) && !user.Phone.Equals("string"))
                {
                    exist.Phone = user.Phone;
                }
                if (!String.IsNullOrEmpty(user.FirstName) && !user.FirstName.Equals("string"))
                {
                    exist.FirstName = user.FirstName;
                }
                if (!String.IsNullOrEmpty(user.LastName) && !user.LastName.Equals("string"))
                {
                    exist.LastName = user.LastName;
                }


                _unitOfWork.UserRepo.Update(exist);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user;
        }
    }
}
