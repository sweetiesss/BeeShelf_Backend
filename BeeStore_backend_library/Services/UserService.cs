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
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.UserEmailDuplicate);
            }
            var result = _mapper.Map<User>(user);
            result.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _unitOfWork.UserRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return user;
        }

        public async Task<string> DeleteUser(int id)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist != null)
            {
                _unitOfWork.UserRepo.SoftDelete(exist);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            return ResponseMessage.Success;
        }

        public async Task<Pagination<UserListDTO>> GetAllUser(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.UserRepo.GetAllAsync();
            if (list == null)
            {
                //implement later
            }

            var result = _mapper.Map<List<UserListDTO>>(list);


            return await ListPagination<UserListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<UserListDTO> GetUser(int id)
        {
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var result = _mapper.Map<UserListDTO>(user);
            return result;
        }

        public async Task<UserListDTO> Login(string email, string password)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == email);
            if (exist != null)
            {
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(password, exist.Password))
                    {
                        return _mapper.Map<UserListDTO>(exist);
                    }
                    else
                    {
                        throw new KeyNotFoundException(ResponseMessage.UserPasswordError);
                    }
                }
                catch (Exception)
                {
                    throw new KeyNotFoundException(ResponseMessage.UserPasswordError);
                }
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }
        }

        public async Task<UserUpdateRequestDTO> UpdateUser(UserUpdateRequestDTO user)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                if (!String.IsNullOrEmpty(user.Password) && !user.Password.Equals("string"))
                {
                    exist.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
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
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }

            return user;
        }
    }
}
