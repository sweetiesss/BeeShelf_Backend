using BeeStore_Repository.Data;
using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public UserController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var listUser = await _unitOfWork.UserRepo.GetQueryable();
            
            return Ok(listUser);
        }
    }
}
