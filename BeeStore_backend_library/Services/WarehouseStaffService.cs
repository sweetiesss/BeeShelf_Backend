using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class WarehouseStaffService : IWarehouseStaffService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WarehouseStaffService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.WarehouseStaff.GetAllAsync();
            var result = _mapper.Map<List<WarehouseStaffListDTO>>(list);
            return (await ListPagination<WarehouseStaffListDTO>.PaginateList(result, pageIndex, pageSize));
        }
    }
}
