using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<Pagination<InventoryListDTO>> GetInventoryList(int pageIndex, int pageSize);
        Task<InventoryCreateDTO> CreateInventory(InventoryCreateDTO request);
        Task<InventoryUpdateDTO> UpdateInventory(InventoryUpdateDTO request);
        Task<string> DeleteInventory(int id);
        Task<InventoryListDTO> AddPartnerToInventory(int id, int userId);
    }
}
