using BeeStore_Repository.DTO.WalletDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDTO> GetWalletByUserId(int userId);
    }
}
