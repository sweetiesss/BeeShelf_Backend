using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    [ApiController]
    public class TestController : BaseController
    {
        [HttpGet]
        [ValidateAntiForgeryToken]
        public IActionResult Index()
        {
            return Ok("Hello");
        }
    }
}
