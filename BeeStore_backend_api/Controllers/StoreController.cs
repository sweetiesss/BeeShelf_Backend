using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class StoreController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IStoreShipperService _storeShipperService;
        private readonly IStoreStaffService _storeStaffService;
        private readonly IMemoryCache _memoryCache;
        private const string storeCacheKey = "allStoreCache";
        private const string categoryCacheKey = "allWarehouseCategoryCache";


        public StoreController(IStoreService storeService,
                                    IStoreShipperService storeShipperService,
                                    IStoreStaffService storeStaffService,
                                    IMemoryCache memoryCache)
        {
            _storeService = storeService;
            _storeShipperService = storeShipperService;
            _storeStaffService = storeStaffService;
            _memoryCache = memoryCache;
        }

        [Route("get-stores")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetStoreList([FromQuery][DefaultValue(null)] string? search,
                                                          [FromQuery] StoreFilter? filterBy, string? filterQuery,
                                                          [FromQuery] StoreSortBy? sortCriteria,
                                                          [FromQuery][DefaultValue(false)] bool descending,
                                                          [FromQuery][DefaultValue(0)] int pageIndex,
                                                          [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _storeService.GetStoreList(search, filterBy, filterQuery, sortCriteria, descending, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("get-store/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner")]
        public async Task<IActionResult> GetStore(int id)
        {
            var result = await _storeService.GetStoreById(id);
            return Ok(result);
        }

        [Route("get-store-by-user/{userId}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff,Partner,Shipper")]
        public async Task<IActionResult> GetStoreByUserId(int userId)
        {
            var result = await _storeService.GetStoreByUserId(userId);
            return Ok(result);
        }


        [Route("get-store-shippers")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetStoreShipperList([FromQuery][DefaultValue(null)] string? search,
                                                                 [FromQuery] bool? hasDeliveryZone,
                                                                 [FromQuery] bool? hasVehicle,
                                                                 [FromQuery] StoreFilter? filterBy,
                                                                 [FromQuery][DefaultValue(null)] string? filterQuery,
                                                                 [FromQuery][DefaultValue(0)] int pageIndex,
                                                                 [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _storeShipperService.GetStoreShipperList(search,hasDeliveryZone, hasVehicle, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }


        [Route("get-store-shippers/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetStoreShipperList(int id,
                                                                 [FromQuery][DefaultValue(null)] string? search, 
                                                                 [FromQuery] bool? hasDeliveryZone,
                                                                 [FromQuery] bool? hasVehicle,
                                                                 [FromQuery] StoreFilter? filterBy,
                                                                 [FromQuery][DefaultValue(null)] string? filterQuery,
                                                                 [FromQuery][DefaultValue(0)] int pageIndex,
                                                                 [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _storeShipperService.GetStoreShipperList(id, search,hasDeliveryZone, hasVehicle, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("add-shippers-to-store")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddStoreShipper(List<StoreShipperCreateDTO> request)
        {
            var result = await _storeShipperService.AddShipperToStore(request);
            return Ok(result);
        }

        [Route("assign-shipper-to-delivery-zone/{shipperId}/{deliveryzoneId}")]
        [HttpPost]
        [Authorize(Roles = "Admin, Manager, Staff")]
        public async Task<IActionResult> AssingnShipperToDeliveryZone(int shipperId, int deliveryzoneId)
        {
            var result = await _storeShipperService.AssignShipperToDeliveryZone(shipperId, deliveryzoneId);
            return Ok(result);
        }

        [Route("get-store-staffs")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetStoreStaffList([FromQuery][DefaultValue(null)] string? search,
                                                               [FromQuery] StoreFilter? filterBy,
                                                               [FromQuery][DefaultValue(null)] string? filterQuery,
                                                               [FromQuery][DefaultValue(0)] int pageIndex,
                                                               [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _storeStaffService.GetStoreStaffList(search, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }


        [Route("get-store-staffs/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetStoreStaffList(int id,
                                                              [FromQuery][DefaultValue(null)] string? search,
                                                              [FromQuery] StoreFilter? filterBy,
                                                              [FromQuery][DefaultValue(null)] string? filterQuery,
                                                              [FromQuery][DefaultValue(0)] int pageIndex,
                                                              [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _storeStaffService.GetStoreStaffList(id, search, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }

        [Route("add-staffs-to-store")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddStoreStaff(List<StoreStaffCreateDTO> request)
        {
            var result = await _storeStaffService.AddStaffToStore(request);
            return Ok(result);
        }



        [Route("create-store")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateStore(StoreCreateDTO request)
        {
            var result = await _storeService.CreateStore(request);
            return Ok(result);
        }


        [Route("update-store/{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStore(int id, StoreCreateDTO request)
        {
            var result = await _storeService.UpdateStore(id, request);
            return Ok(result);
        }

        [Route("delete-store/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var result = await _storeService.DeleteStore(id);
            return Ok(result);
        }
    }
}
