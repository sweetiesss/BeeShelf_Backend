using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PackageDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPackageService
    {
        Task<Pagination<PackageListDTO>> GetPackageList(int pageIndex, int pageSize);
        Task<PackageListDTO> GetPackageById(int id);
        Task<PackageCreateDTO> CreatePackage(PackageCreateDTO request);
        Task<PackageCreateDTO> UpdatePackage(int id, PackageCreateDTO request);
        Task<string> DeletePackage(int id);
    }
}
