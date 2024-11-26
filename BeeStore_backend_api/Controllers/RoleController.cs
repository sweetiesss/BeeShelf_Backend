using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Route("get-roles")]
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _roleService.GetRoles();
            return Ok(result);
        }


        [Route("update-user-role/{userId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserRole(int userId, string roleName)
        {
            var result = await _roleService.UpdateUserRole(userId, roleName);
            return Ok(result);
        }


    }
}
