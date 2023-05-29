using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ClientDtos;

namespace RushHour.Domain.Abstractions.Services
{
    public interface IClientService
    {
        Task<GetClientDto> CreateClientAsync(CreateClientDto dto);
        Task UpdateClientAsync(Guid id, UpdateClientDto dto, Guid requesterId);
        Task<GetClientDto> GetClientByIdAsync(Guid requesterId, Guid id);
        Task<PaginatedResult<GetClientDto>> GetPageAsync(int index, int pageSize);
        Task<GetClientDto> GetClientByAccountAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
