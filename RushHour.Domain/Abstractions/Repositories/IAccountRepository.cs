using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IAccountRepository
    {
        Task<AccountDto> CreateAsync(CreateAccountDto dto);
        Task UpdateAsync(Guid id, CreateAccountDto dto);
        Task<AccountDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<AccountDto>> GetPageAsync(int index, int pageSize);
        Task DeleteAsync(Guid id);
    }
}
