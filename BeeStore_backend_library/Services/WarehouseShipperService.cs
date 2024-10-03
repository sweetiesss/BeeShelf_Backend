using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class WarehouseShipperService : IWarehouseShipperService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WarehouseShipperService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Pagination<WarehouseShipperListDTO>> GetWarehouseShipperList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.WarehouseShipper.GetAllAsync();
            var result = _mapper.Map<List<WarehouseShipperListDTO>>(list);
            return (await ListPagination<WarehouseShipperListDTO>.PaginateList(result, pageIndex, pageSize));
        }
    }
}
