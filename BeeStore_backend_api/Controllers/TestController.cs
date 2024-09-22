using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    [ApiController]
    public class TestController : BaseController
    {

        private readonly IJWTService _jwtService;

        public TestController(IJWTService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello!");
        }


        [Authorize]
        [HttpGet("secured")]
        public IActionResult SecureIndex()
        {
            return Ok("Hello, this is a secure endpoint!");
        }

        [HttpGet("token")]
        public IActionResult GetToken()
        {
            var token = _jwtService.GenerateJwtToken("Test user");
            return Ok(new { Token = token });
        }
    }
}
