using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using System.Text;

namespace BeeStore_Repository.Services
{
    public class WarehouseCategoryService : IWarehouseCategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WarehouseCategoryService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<WarehouseCategoryCreateDTO>> AddCategoryToWarehouse(List<WarehouseCategoryCreateDTO> request)
        {
            StringBuilder sb = new StringBuilder();
            string error = string.Empty;

            //check dupes in list
            var dupes = request.Select((x, i) => new { index = i, value = x })
                   .GroupBy(x => new { x.value.ProductCategoryId })
                   .Where(x => x.Skip(1).Any());

            if (dupes.Any())
            {
                foreach (var group in dupes)
                {
                    var a = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id == group.Key.ProductCategoryId);
                    sb.Append($"{a.TypeName}, ");
                }
                error = sb.ToString();
                if (!String.IsNullOrEmpty(error))
                {
                    throw new DuplicateException(ResponseMessage.WarehouseCategoryDuplicateList + error);
                }
            }

            foreach (var item in request)
            {
                if ((await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id == item.ProductCategoryId)) == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound + $": {item.ProductCategoryId}");
                }

                var exist = await _unitOfWork.WarehouseCategoryRepo.FirstOrDefaultAsync(u => u.WarehouseId == item.WarehouseId && u.ProductCategoryId == item.ProductCategoryId);
                if (exist != null)
                {
                    if (exist.IsDeleted == true)
                    {
                        exist.WarehouseId = null;
                        _unitOfWork.WarehouseCategoryRepo.Update(exist);
                        await _unitOfWork.SaveAsync();
                    }
                    else
                    {
                        var a = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id == item.ProductCategoryId);
                        sb.Append($"{a.TypeName}, ");
                    }
                }
            }
            error = sb.ToString();
            if (!String.IsNullOrEmpty(error))
            {
                throw new DuplicateException(ResponseMessage.WarehouseCategoryAddListFailed + error);
            }
            var result = _mapper.Map<List<WarehouseCategory>>(request);
            await _unitOfWork.WarehouseCategoryRepo.AddRangeAsync(result);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<Pagination<WarehouseCategoryListDTO>> GetWarehouseCategoryList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.WarehouseCategoryRepo.GetAllAsync();
            var result = _mapper.Map<List<WarehouseCategoryListDTO>>(list);
            return (await ListPagination<WarehouseCategoryListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<WarehouseCategoryListDTO>> GetWarehouseCategoryList(int id, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.WarehouseCategoryRepo.GetFiltered(u => u.WarehouseId == id);
            var result = _mapper.Map<List<WarehouseCategoryListDTO>>(list);
            return (await ListPagination<WarehouseCategoryListDTO>.PaginateList(result, pageIndex, pageSize));
        }
    }
}
