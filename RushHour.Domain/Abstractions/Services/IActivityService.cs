using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ActivityDtos;

namespace RushHour.Domain.Abstractions.Services
{
    public interface IActivityService
    {
        Task<GetActivityDto> CreateActivityAsync(Guid requesterAccountId, CreateActivityDto dto);
        Task UpdateActivityAsync(Guid id, CreateActivityDto dto, Guid requesterAccountId);
        Task<GetActivityDto> GetActivityByIdAsync(Guid requesterAccountId, Guid id);
        Task<PaginatedResult<GetActivityDto>> GetPageAsync(int index, int pageSize, Guid requesterAccountId);
        Task DeleteAsync(Guid id);
    }
}
