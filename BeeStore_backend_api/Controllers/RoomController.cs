using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class RoomController : BaseController
    {
        private readonly IRoomService _roomService;
        private readonly IMemoryCache _memoryCache;
        private const string cacheKey = "inventoryCache";
        public RoomController(IRoomService roomService, IMemoryCache memoryCache)
        {
            _roomService = roomService;
            _memoryCache = memoryCache;
        }

        [Route("buy-room/{id}/{userId}")]
        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> BuyRoom(int id, int userId, int month)
        {
            var result = await _roomService.BuyRoom(id, userId, month);
            return Ok(result);
        }

        [Route("extend-room/{id}/{userId}")]
        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> ExtendRoom(int id, int userId, int month)
        {
            var result = await _roomService.ExtendRoom(id, userId, month);
            return Ok(result);
        }

        [Route("get-rooms")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetRoomList([FromQuery] RoomFilter? filterBy,
                                                          [FromQuery][DefaultValue(null)] string? filterQuery,
                                                          [FromQuery] RoomSortBy? sortCriteria,
                                                          [FromQuery][DefaultValue(false)] bool descending,
                                                          [FromQuery][DefaultValue(0)] int pageIndex,
                                                          [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _roomService.GetRoomList(filterBy, filterQuery, sortCriteria, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-rooms/{userId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetRoomList(int userId,
                                                          [FromQuery] RoomFilter? filterBy,
                                                          [FromQuery][DefaultValue(null)] string? filterQuery,
                                                          [FromQuery] RoomSortBy? sortCriteria,
                                                          [FromQuery][DefaultValue(false)] bool descending,
                                                          [FromQuery][DefaultValue(0)] int pageIndex,
                                                          [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _roomService.GetRoomList(userId, filterBy, filterQuery, sortCriteria, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-room/{id}")]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        [HttpGet]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var result = await _roomService.GetRoomById(id);
            return Ok(result);
        }


        [Route("create-room")]
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomCreateDTO request)
        {
            var result = await _roomService.CreateRoom(request);
            return Ok(result);
        }

        [Route("update-room")]
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut]
        public async Task<IActionResult> UpdateRoom(RoomUpdateDTO request)
        {
            var result = await _roomService.UpdateRoom(request);
            return Ok(result);
        }

        [Route("delete-room/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var result = await _roomService.DeleteRoom(id);
            return Ok(result);
        }
    }

}
