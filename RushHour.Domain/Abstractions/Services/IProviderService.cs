using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ProviderDtos;

namespace RushHour.Domain.Abstractions.Services
{
    public interface IProviderService
    {
        Task<GetProviderDto> CreateAsync(CreateProviderDto dto);
        Task UpdateAsync(Guid requesterId, Guid id, CreateProviderDto dto);
        Task<GetProviderDto> GetByIdAsync(Guid requesterId, Guid id);
        Task<PaginatedResult<GetProviderDto>> GetPageAsync(int index, int pageSize);
        Task DeleteAsync(Guid id);
    }
}
