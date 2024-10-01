using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryList([FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _inventoryService.GetInventoryList(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost("{id}/{email}")]
        public async Task<IActionResult> AddPartnerToInventory(int id, string email)
        {
            var result = await _inventoryService.AddPartnerToInventory(id, email);
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
