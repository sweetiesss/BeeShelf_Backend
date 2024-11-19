using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.Batch;
using BeeStore_Repository.Enums.FilterBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IBatchService
    {
        Task<Pagination<BatchListDTO>> GetBatchList(string search, BatchFilter? filterBy, string? filterQuery, int pageIndex, int pageSize);

        Task<string> CreateBatch(BatchCreateDTO request);
        Task<string> DeleteBatch(int id);
        Task<string> UpdateBatch(int id, BatchCreateDTO request);
        Task<string> AssignBatch(int id, int shipperId);
    }
}
