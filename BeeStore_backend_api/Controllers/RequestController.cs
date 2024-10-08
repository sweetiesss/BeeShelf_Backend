using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> GetRequestList([FromQuery][DefaultValue(0)] int pageIndex,
                                                    [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _requestService.GetRequestList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetRequestList(string email, [FromQuery][DefaultValue(0)] int pageIndex,
                                                    [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _requestService.GetRequestList(email, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(RequestCreateDTO request)
        {
            var result = await _requestService.CreateRequest(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequest(int id, RequestCreateDTO request)
        {
            var result = await _requestService.UpdateRequest(id, request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequestStatus(int id, int statusId)
        {
            var result = await _requestService.UpdateRequestStatus(id, statusId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var result = await _requestService.DeleteRequest(id);
            return Ok(result);
        }

    }
}
