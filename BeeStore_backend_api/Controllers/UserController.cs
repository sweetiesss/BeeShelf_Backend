using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{

    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IJWTService _jwtService;

        public UserController(IUserService userService, IJWTService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [Route("get-users")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers([FromQuery][DefaultValue(null)] string? search,
                                                  [FromQuery] UserSortBy? sortBy,
                                                  [FromQuery] UserRole? filterByRole,
                                                  [FromQuery][DefaultValue(false)] bool descending,
                                                  [FromQuery][DefaultValue(0)] int pageIndex,
                                                  [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _userService.GetAllUser(search!, filterByRole, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-user/{email}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner,Shipper")]
        public async Task<IActionResult> GetUser(string email)
        {
            var result = await _userService.GetUser(email);
            return Ok(result);
        }

        [Route("create-user")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequestDTO user)
        {
            var result = await _userService.CreateUser(user);
            return Ok(result);
        }

        [Route("update-user")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager,Staff,Partner,Shipper")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequestDTO user)
        {
            var result = await _userService.UpdateUser(user);
            return Ok(result);
        }


        [Route("delete-user/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            return Ok(result);
        }
    }
}
