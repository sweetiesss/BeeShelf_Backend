﻿using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IJWTService _jwtService;
        private readonly IUserService _userService;

        public AuthController(IJWTService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO request)
        {
            var result = await _userService.Login(request.email, request.password);

            return Ok(_jwtService.GenerateJwtToken(result.Email, result.RoleName));
        }

        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] UserCreateRequestDTO user)
        {
            var result = await _userService.CreateUser(user);
            return Ok(result);
        }
    }
}
