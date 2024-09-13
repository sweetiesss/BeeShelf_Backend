using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    [ApiController]
    public class TestController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello");
        }
    }
}
