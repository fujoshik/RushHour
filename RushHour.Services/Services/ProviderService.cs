using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ProviderDtos;

namespace RushHour.Services.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _repository;

        public ProviderService(IProviderRepository repository)
        {
            this._repository = repository;
        }

        public async Task<GetProviderDto> CreateAsync(CreateProviderDto dto)
        {
            var provider = await _repository.CreateAsync(dto);

            return provider;
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            await _repository.DeleteAsync(id);
        }

        public async Task<PaginatedResult<GetProviderDto>> GetPageAsync(int index, int pageSize)
        {
            return await _repository.GetPageAsync(index, pageSize);
        }

        public async Task<GetProviderDto> GetByIdAsync(Guid id)
        {
            GetProviderDto provider = await _repository.GetByIdAsync(id);

            return provider;
        }

        public async Task UpdateAsync(Guid id, CreateProviderDto dto)
        {
            await _repository.UpdateAsync(id, dto);
        }
    }
}
