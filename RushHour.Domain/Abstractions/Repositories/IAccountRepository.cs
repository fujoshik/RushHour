using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.Enums;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IAccountRepository
    {
        Task<AccountDto> CreateAsync(CreateAccountDto dto, byte[] salt);
        Task UpdateAsync(Guid id, CreateAccountDto dto);
        Task<AccountDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<AccountDto>> GetPageAsync(int index, int pageSize);
        Task DeleteAsync(Guid id);
        Task<bool> CheckIfAnyMatchesIdAndRole(Guid id, Role role);
        Task<List<AccountDto>> GetUsersByEmail(string email);
    }
}
