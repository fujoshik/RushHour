using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ClientDtos;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IClientRepository
    {
        Task<GetClientDto> CreateAsync(GetAccountDto account, CreateClientDto client);
        Task UpdateAsync(Guid id, UpdateClientDto dto);
        Task<GetClientDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<GetClientDto>> GetPageAsync(int index, int pageSize);
        Task DeleteAsync(Guid id);
    }
}
