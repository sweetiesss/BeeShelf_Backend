using Microsoft.AspNetCore.Mvc;

namespace BeeStore_backend_api.Controllers
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
