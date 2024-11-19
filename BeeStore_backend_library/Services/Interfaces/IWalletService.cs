using BeeStore_Repository.DTO.WalletDTO;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDTO> GetWalletByUserId(int userId);
    }
}
