using BeeStore_Repository.DTO;

namespace BeeStore_Repository.Utils
{
    public static class ListPagination<TEntity>
    {
        public static Task<Pagination<TEntity>> PaginateList(List<TEntity> list, int pageIndex = 0, int pageSize = 10)
        {
            if (pageSize <= 0)
            {
                throw new ApplicationException(ResponseMessage.InvalidPageSize);
            }
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
