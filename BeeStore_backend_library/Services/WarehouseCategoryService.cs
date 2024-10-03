using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Pagination<WarehouseCategoryListDTO>> GetWarehouseCategoryList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.WarehouseCategory.GetAllAsync();
            var result = _mapper.Map<List<WarehouseCategoryListDTO>>(list);
            return (await ListPagination<WarehouseCategoryListDTO>.PaginateList(result, pageIndex, pageSize));
        }
    }
}
