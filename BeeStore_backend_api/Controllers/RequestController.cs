using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class RequestController : BaseController
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [Route("get-requests")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetRequestList([FromQuery][DefaultValue(0)] int pageIndex,
                                                    [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _requestService.GetRequestList(pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-requests/{userId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetRequestList(int userId, [FromQuery][DefaultValue(0)] int pageIndex,
                                                    [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _requestService.GetRequestList(userId, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("create-request")]
        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> CreateRequest(RequestCreateDTO request)
        {
            var result = await _requestService.CreateRequest(request);
            return Ok(result);
        }

        [Route("update-request/{id}")]
        [HttpPut]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> UpdateRequest(int id, RequestCreateDTO request)
        {
            var result = await _requestService.UpdateRequest(id, request);
            return Ok(result);
        }

        [Route("update-request-status/{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> UpdateRequestStatus(int id, int statusId)
        {
            var result = await _requestService.UpdateRequestStatus(id, statusId);
            return Ok(result);
        }

        [Route("delete-request/{id}")]
        [Authorize(Roles = "Partner")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var result = await _requestService.DeleteRequest(id);
            return Ok(result);
        }

    }
}
