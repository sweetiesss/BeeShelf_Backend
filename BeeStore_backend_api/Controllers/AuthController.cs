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

            if (result == null)
            {
                return Unauthorized();
            }

            return Ok(_jwtService.GenerateJwtToken(result.email, result.role));
        }

        [Route("sign-up")]
        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] UserSignUpRequestDTO user)
        {
            var result = await _userService.SignUp(user);
            return Ok(result);
        }

        [Route("refresh-token")]
        [HttpPost]
        public IActionResult RefreshToken([FromBody] UserRefreshTokenRequestDTO jwt)
        {
            return Ok(_jwtService.RefreshJWTToken(jwt));
        }

        [Route("forgot-password")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _userService.ForgotPassword(email);
            return Ok(result);
        }

        [Route("reset-password")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(UserForgotPasswordRequest request)
        {
            var result = await _userService.ResetPassword(request);
            return Ok(result);
        }
    }
}
