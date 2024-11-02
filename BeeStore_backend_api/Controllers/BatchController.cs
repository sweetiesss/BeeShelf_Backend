using Amazon.S3.Model;
using BeeStore_Repository.DTO.Batch;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BeeStore_Api.Controllers
{
    public class BatchController : BaseController
    {
        private readonly IBatchService _batchService;

        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [Route("create-batch")]
        [HttpPost]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> CreateBatch(BatchCreateDTO request)
        {
            var result = await _batchService.CreateBatch(request);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager,Staff")]
        [Route("get-batches")]
        [HttpGet]
        public async Task<IActionResult> GetAllBatches([FromQuery] string? search,
                                                       [FromQuery] BatchFilter? filterBy,
                                                       [FromQuery][DefaultValue(null)] string? filterQuery,
                                                       [FromQuery][DefaultValue(0)] int pageIndex,
                                                       [FromQuery][DefaultValue(10)] int pageSize)
        {
            var result = await _batchService.GetBatchList(search, filterBy, filterQuery, pageIndex, pageSize);
            return Ok(result);
        }
    }
}
