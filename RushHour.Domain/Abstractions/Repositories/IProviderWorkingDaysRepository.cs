using RushHour.Domain.DTOs.ProviderWorkingDaysDto;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IProviderWorkingDaysRepository
    {
        Task CreateProviderWithManyWorkingDaysAsync(Guid providerId, List<DayOfWeek> workingDays);
        Task<List<ProviderWorkingDaysDto>> GetAllWorkingDaysOfProviderAsync(Guid providerId);
        Task DeleteProviderWithManyWorkingDaysAsync(Guid providerId, List<DayOfWeek> workingDays = null);
    }
}
