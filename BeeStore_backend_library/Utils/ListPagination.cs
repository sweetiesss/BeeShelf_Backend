using BeeStore_Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils
{
    public static class ListPagination<TEntity>
    {
        public static Task<Pagination<TEntity>> PaginateList(List<TEntity> list, int pageIndex = 0, int pageSize = 10)
        {
            var itemCount = list.Count;
            var items = list.Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();

            var result = new Pagination<TEntity>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = itemCount,
                Items = items,
            };

            return Task.FromResult(result);
        }
    }
}
