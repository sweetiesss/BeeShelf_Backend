using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> GetUsers([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _userService.GetAllUser(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-user/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _userService.GetUser(id);
            return Ok(result);
        }

        [Route("create-user")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]UserCreateRequestDTO user)
        {
            var result = await _userService.CreateUser(user);
            return Ok(result);
        }

        [Route("update-user")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody]UserUpdateRequestDTO user)
        {
            var result = await _userService.UpdateUser(user);
            return Ok(result);
        }

        [Route("delete-user/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            return Ok(result);
        }

                                                    
    }
}
