using BeeStore_Repository.DTO.UserDTOs;
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

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO request)
        {
            var result = await _userService.Login(request.email, request.password);

            return Ok(_jwtService.GenerateJwtToken(result.Email, result.RoleName));
        }

        [Route("sign-up")]
        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] UserCreateRequestDTO user)
        {
            // optimize EX here: it should be the same for the rest of the API
            // You dont need to return DTO here. Instead, You should just return status code and no data
            // the FE are not likely to use this response data anyway
            // We can check whether the user have been created or not internally
            // less response = faster API
            // Also, change to UserRegisterRequestDTO
            var result = await _userService.CreateUser(user);
            return Ok(result);
        }
    }
}
