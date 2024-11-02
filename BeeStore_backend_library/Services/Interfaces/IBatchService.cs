using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.Batch;
using BeeStore_Repository.Enums.FilterBy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IBatchService
    {
        Task<Pagination<BatchListDTO>> GetBatchList(string search, BatchFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);
        
            Task<string> CreateBatch(BatchCreateDTO request);
    }
}
