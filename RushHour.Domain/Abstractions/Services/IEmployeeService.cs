using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.EmployeeDtos;

namespace RushHour.Domain.Abstractions.Services
{
    public interface IEmployeeService
    {      
        Task<GetEmployeeDto> CreateEmployeeAsync(Guid requesterId, CreateEmployeeDto dto);
        Task UpdateEmployeeAsync(Guid id, CreateEmployeeDto dto, Guid requesterId);
        Task<GetEmployeeDto> GetEmployeeByIdAsync(Guid requesterId, Guid id);     
        Task<PaginatedResult<GetEmployeeDto>> GetPageAsync(int index, int pageSize, Guid requesterId);
        Task DeleteAsync(Guid id);
    }
}
