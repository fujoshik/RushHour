using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.EmployeeDtos;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IEmployeeRepository
    {
        Task<GetEmployeeDto> CreateAsync(GetAccountDto account, CreateEmployeeDto employee);
        Task UpdateAsync(Guid id, CreateEmployeeDto dto);
        Task<GetEmployeeDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<GetEmployeeDto>> GetPageAsync(int index, int pageSize);
        Task DeleteAsync(Guid id);
    }
}
