using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IRequestService
    {
        Task<Pagination<RequestListDTO>> GetRequestList(int pageIndex, int pageSize);
        Task<Pagination<RequestListDTO>> GetRequestList(int userId, int pageIndex, int pageSize);

        Task<string> UpdateRequestStatus(int id, int statusId);
        Task<string> CreateRequest(RequestCreateDTO request);
        Task<string> UpdateRequest(int id, RequestCreateDTO request);
        Task<string> DeleteRequest(int id);
    }
}
