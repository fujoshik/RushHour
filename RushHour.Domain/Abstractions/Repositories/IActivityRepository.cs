using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ActivityDtos;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IActivityRepository
    {
        Task<GetActivityDto> CreateAsync(CreateActivityDto dto);
        Task UpdateAsync(Guid id, CreateActivityDto dto);
        Task<GetActivityDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<GetActivityDto>> GetPageAsync(int index, int pageSiz, Guid requesterProviderId = default(Guid));
        Task DeleteAsync(Guid id);
    }
}
