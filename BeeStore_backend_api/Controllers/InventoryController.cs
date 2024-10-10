using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMemoryCache _memoryCache;
        private const string cacheKey = "inventoryCache";
        public InventoryController(IInventoryService inventoryService, IMemoryCache memoryCache)
        {
            _inventoryService = inventoryService;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetInventoryList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _inventoryService.GetInventoryList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("{email}")]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetInventoryList(string email, [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out var result)) // result here have the variable types as Pagination<InventoryListDTO>
            {
               result = await _inventoryService.GetInventoryList(email, pageIndex, pageSize);

               var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                   .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _memoryCache.Set(cacheKey, result, cacheEntryOptions);
            }
            return Ok(result);
        }

        [HttpPost("{id}/{userId}")]
        public async Task<IActionResult> AddPartnerToInventory(int id, int userId)
        {
            var result = await _inventoryService.AddPartnerToInventory(id, userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory(InventoryCreateDTO request)
        {
            var result = await _inventoryService.CreateInventory(request);
            return Ok(result);  
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInventory(InventoryUpdateDTO request)
        {
            var result = await _inventoryService.UpdateInventory(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var result = await _inventoryService.DeleteInventory(id);
            return Ok(result);
        }
    }

}
