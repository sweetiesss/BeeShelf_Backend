using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Enums;
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
        public async Task<IActionResult> GetRequestList([FromQuery] RequestStatus? status,
                                                    [FromQuery] bool? import,
                                                    [FromQuery][DefaultValue(false)] bool descending,
                                                    [FromQuery] int warehouseId,
                                                    [FromQuery][DefaultValue(0)] int pageIndex,
                                                    [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _requestService.GetRequestList(import, status, descending, warehouseId, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-requests/{userId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetRequestList(int userId,
                                                    [FromQuery][DefaultValue(false)] bool descending,
                                                    [FromQuery] bool? import,
                                                    [FromQuery] RequestStatus? status,
                                                    [FromQuery][DefaultValue(0)] int pageIndex,
                                                    [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _requestService.GetRequestList(userId, import, status, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("create-request")]
        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> CreateRequest(RequestType type,
                                                       [DefaultValue(false)] bool send,
                                                       RequestCreateDTO request)
        {
            var result = await _requestService.CreateRequest(type, send, request);
            return Ok(result);
        }

        [Route("send-request")]
        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> SendRequest(int id)
        {
            var result = await _requestService.SendRequest(id);
            return Ok(result);
        }

        [Route("cancel-request")]
        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> CancelRequest(int id, string? cancellationReason)
        {
            var result = await _requestService.CancelRequest(id, cancellationReason);
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
        public async Task<IActionResult> UpdateRequestStatus(int id, RequestStatus status)
        {
            var result = await _requestService.UpdateRequestStatus(id, status);
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
