using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IRoomService
    {
        Task<Pagination<RoomListDTO>> GetRoomList(RoomFilter? filterBy, string filterQuery,
                                                          RoomSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize);
        Task<Pagination<RoomListDTO>> GetRoomList(int userId, RoomFilter? filterBy, string filterQuery,
                                                          RoomSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize);
        Task<RoomLotListDTO> GetRoomById(int id);

        Task<string> CreateRoom(RoomCreateDTO request);
        Task<string> UpdateRoom(RoomUpdateDTO request);
        Task<string> DeleteRoom(int id);
        Task<string> BuyRoom(int id, int userId, int month);
        Task<string> ExtendRoom(int id, int userId, int month);
    }
}
