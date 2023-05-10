using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ActivityEmployeeDtos;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IActivityEmployeeRepository
    {
        Task<ActivityEmployeeDto> CreateAsync(ActivityEmployeeDto actEmp);
        Task CreateActivityWithManyEmployeesAsync(Guid activityId, List<Guid> employeeIds);
        Task<List<ActivityEmployeeDto>> GetAllEmployeesOfActivityAsync(Guid activityId);
        Task<PaginatedResult<ActivityEmployeeDto>> GetPageAsync(int index, int pageSize, Guid activityId = default(Guid));
        Task<ActivityEmployeeDto> GetByActivityAndEmployeeIdAsync(Guid activityId, Guid employeeId);
        Task DeleteAsync(Guid activityId, Guid employeeId);
        Task DeleteActivityWithManyEmployeesAsync(Guid activityId, List<Guid> employeeIds = null);
    }
}
