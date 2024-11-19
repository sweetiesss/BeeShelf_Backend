using AutoMapper;
using BeeStore_Repository.DTO.WalletDTO;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;

namespace BeeStore_Repository.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public WalletService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<WalletDTO> GetWalletByUserId(int userId)
        {
            var wallet = await _unitOfWork.WalletRepo.SingleOrDefaultAsync(u => u.OcopPartnerId.Equals(userId));
            if (wallet == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WalletIdNotFound);
            }
            var result = _mapper.Map<WalletDTO>(wallet);
            return result;
        }
    }
}
