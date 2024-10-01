using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomProductCategoryResolverProduct : IValueResolver<Product, ProductListDTO, string?>
    {
        private readonly BeeStoreDbContext _context;

        public CustomProductCategoryResolverProduct(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Product source, ProductListDTO destination, string destMember, ResolutionContext context)
        {
            var proCat = _context.ProductCategories.FirstOrDefault(r => r.Id == source.ProductCategoryId);
            return proCat != null ? proCat.TypeName : null;
        }
    }
}
