using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ProviderDtos;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IProviderRepository
    {
        Task<GetProviderDto> CreateAsync(CreateProviderDto dto);
        Task UpdateAsync(GetProviderDto dto);
        Task<GetProviderDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<GetProviderDto>> GetAllAsync(int index, int pageSize);
        Task DeleteAsync(Guid id);
    }
}
