using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface ILotService
    {
        Task<Pagination<LotListDTO>> GetAllLots(string search, LotFilter? filterBy, string? filterQuery, 
                                                        LotSortBy? sortBy, bool descending, 
                                                        int pageIndex, int pageSize);
        Task<Pagination<LotListDTO>> GetLotsByUserId(int parnerId, string search, LotFilter? filterBy, string? filterQuery,
                                                        LotSortBy? sortBy, bool descending,
                                                        int pageIndex, int pageSize);
        Task<LotListDTO> GetLotById(int id);
        //Task<string> CreateLot(LotCreateDTO request);
        //Task<string> UpdateLot(int id, LotCreateDTO request);
        Task<string> DeleteLot(int id);
    }
}
