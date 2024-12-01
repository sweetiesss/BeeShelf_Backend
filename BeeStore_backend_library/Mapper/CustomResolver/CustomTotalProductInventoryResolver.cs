using AutoMapper;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{

    public class CustomTotalProductInventoryResolver : IValueResolver<Inventory, InventoryListDTO, int>
    {
        public int Resolve(Inventory source, InventoryListDTO destination, int destMember, ResolutionContext context)
        {
            // Sum the product amounts of all lots associated with this inventory
            return source.Lots?.Where(lot => !lot.IsDeleted).Sum(lot => lot.TotalProductAmount) ?? 0;
        }
    }
}
