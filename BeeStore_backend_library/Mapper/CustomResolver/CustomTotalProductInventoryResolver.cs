using AutoMapper;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Mapper.CustomResolver
{

        public class CustomTotalProductInventoryResolver : IValueResolver<Inventory, InventoryListDTO, int>
        {
            public int Resolve(Inventory source, InventoryListDTO destination, int destMember, ResolutionContext context)
            {
            // Sum the product amounts of all lots associated with this inventory
            return source.Lots?.Where(lot => !lot.IsDeleted).Sum(lot => lot.ProductAmount) ?? 0;
        }
        }
}
