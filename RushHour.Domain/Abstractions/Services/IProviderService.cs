using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ProviderDtos;

namespace RushHour.Domain.Abstractions.Services
{
    public interface IProviderService
    {
        Task<GetProviderDto> CreateAsync(CreateProviderDto entity);
        Task UpdateAsync(GetProviderDto entity);
        Task<GetProviderDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<GetProviderDto>> GetAllAsync(int index, int pageSize);
        Task DeleteAsync(Guid id);
    }
}
