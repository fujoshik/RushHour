using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.EmployeeDtos;

namespace RushHour.Domain.Abstractions.Services
{
    public interface IEmployeeService
    {      
        Task<GetEmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task UpdateEmployeeAsync(Guid id, CreateEmployeeDto dto, string requesterId);
        Task<GetEmployeeDto> GetEmployeeByIdAsync(Guid id);     
        Task<PaginatedResult<GetEmployeeDto>> GetPageAsync(int index, int pageSize);
        Task DeleteAsync(Guid id);
    }
}
