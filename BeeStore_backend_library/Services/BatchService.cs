using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.Batch;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class BatchService : IBatchService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public BatchService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<string> CreateBatch(BatchCreateDTO request)
        {
            foreach (var o in request.Orders)
            {

                if (await _unitOfWork.OrderRepo.AnyAsync(u => u.Id.Equals(o.Id) && u.BatchId != null))
                {
                    throw new ApplicationException(ResponseMessage.OrderBatchError);
                }
                if (await _unitOfWork.OrderRepo.AnyAsync(u => u.Id == o.Id && u.IsDeleted == false) == false)
                {
                    throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
                }

            }
            var result = _mapper.Map<Batch>(request);
            result.Status = Constants.Status.Pending;
            await _unitOfWork.BatchRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            foreach (var o in request.Orders)
            {
                var order = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == o.Id);
                order.BatchId = result.Id;
                await _unitOfWork.SaveAsync();
            }
            return ResponseMessage.Success;
        }

        public async Task<Pagination<BatchListDTO>> GetBatchList(string search, BatchFilter? filterBy, string? filterQuery, int pageIndex, int pageSize)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }

            Expression<Func<Batch, bool>> filterExpression = null;
            switch (filterBy)
            {
                case BatchFilter.DeliveryZoneId: filterExpression = u => u.DeliveryZoneId.Equals(Int32.Parse(filterQuery!)); break;
                case BatchFilter.Status: filterExpression = u => u.Status.Equals(filterQuery!, StringComparison.OrdinalIgnoreCase); break;
                default: filterExpression = null; break;
            }


            var list = await _unitOfWork.BatchRepo.GetListAsync(
                filter: filterExpression!,
                includes: null,
                sortBy: null,
                descending: false,
                searchTerm: search,
                searchProperties: new Expression<Func<Batch, string>>[] { o => o.Name }
                );

            var result = _mapper.Map<List<BatchListDTO>>(list);
            return (await ListPagination<BatchListDTO>.PaginateList(result, pageIndex, pageSize));
        }
    }
}
