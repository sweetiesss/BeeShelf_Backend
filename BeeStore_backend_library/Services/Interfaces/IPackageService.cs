using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPackageService
    {
        Task<Pagination<PackageListDTO>> GetPackageList(PackageFilter filterBy, string? filterQuery, 
                                                        PackageSortBy sortBy, bool descending, 
                                                        int pageIndex, int pageSize);
        Task<PackageListDTO> GetPackageById(int id);
        Task<string> CreatePackage(PackageCreateDTO request);
        Task<string> UpdatePackage(int id, PackageCreateDTO request);
        Task<string> DeletePackage(int id);
    }
}
