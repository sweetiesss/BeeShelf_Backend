using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
    }
}
