using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class LotController : BaseController
    {
        private readonly ILotService _lotService;

        public LotController(ILotService lotService)
        {
            _lotService = lotService;
        }

        [Authorize(Roles = "Admin,Manager")]
        [Route("get-lots")]
        [HttpGet]
        public async Task<IActionResult> GetAllLots([FromQuery] string? search,
                                                        [FromQuery] LotFilter? filterBy,
                                                        [FromQuery][DefaultValue(null)] string? filterQuery,
                                                        [FromQuery] LotSortBy? sortBy,
                                                        [FromQuery][DefaultValue(false)] bool descending,
                                                        [FromQuery][DefaultValue(0)] int pageIndex,
                                                        [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _lotService.GetAllLots(search, filterBy, filterQuery, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Authorize(Roles = "Partner")]
        [Route("get-lots/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetAllLots(int userId,
                                                        [FromQuery] string? search,
                                                        [FromQuery] LotFilter? filterBy,
                                                        [FromQuery][DefaultValue(null)] string? filterQuery,
                                                        [FromQuery] LotSortBy? sortBy,
                                                        [FromQuery][DefaultValue(false)] bool descending,
                                                        [FromQuery][DefaultValue(0)] int pageIndex,
                                                        [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _lotService.GetLotsByUserId(userId, search, filterBy, filterQuery, sortBy, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-lot/{id}")]
        [Authorize(Roles = "Partner")]
        [HttpGet]
        public async Task<IActionResult> GetLotById(int id)
        {
            var result = await _lotService.GetLotById(id);
            return Ok(result);
        }


        [Route("delete-lot/{id}")]
        [Authorize(Roles = "Partner")]
        [HttpDelete]
        public async Task<IActionResult> DeleteLot(int id)
        {
            var result = await _lotService.DeleteLot(id);
            return Ok(result);
        }
    }
}
