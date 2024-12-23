using AutoMapper;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{

    public class CustomTotalProductInventoryResolver : IValueResolver<Room, RoomListDTO, int>
    {
        public int Resolve(Room source, RoomListDTO destination, int destMember, ResolutionContext context)
        {
            // Sum the product amounts of all lots associated with this inventory
            return source.Lots?.Where(lot => !lot.IsDeleted && lot.ImportDate != null).Sum(lot => lot.TotalProductAmount) ?? 0;
        }
    }
}
