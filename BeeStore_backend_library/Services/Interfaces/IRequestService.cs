using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IRequestService
    {
        Task<Pagination<RequestListDTO>> GetRequestList(int pageIndex, int pageSize);
        Task<Pagination<RequestListDTO>> GetRequestList(string email, int pageIndex, int pageSize);

        Task<RequestListDTO> UpdateRequestStatus(int id, int statusId);
        Task<RequestCreateDTO> CreateRequest(RequestCreateDTO request);
        Task<RequestCreateDTO> UpdateRequest(int id, RequestCreateDTO request);
        Task<string> DeleteRequest(int id);        
    }
}
