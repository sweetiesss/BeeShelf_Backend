using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Enums;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IRequestService
    {
        Task<Pagination<RequestListDTO>> GetRequestList(RequestStatus? status,bool descending, int warehouseId, int pageIndex, int pageSize);
        Task<Pagination<RequestListDTO>> GetRequestList(int userId,RequestStatus? status, bool descending, int pageIndex, int pageSize);

        Task<string> UpdateRequestStatus(int id, RequestStatus status);
        Task<string> CancelRequest(int id, string cancellationReason);
        Task<string> SendRequest(int id);
        Task<string> CreateRequest(RequestType type,bool send, RequestCreateDTO request);
        Task<string> UpdateRequest(int id, RequestCreateDTO request);
        Task<string> DeleteRequest(int id);
    }
}
