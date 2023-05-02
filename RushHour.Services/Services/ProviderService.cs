using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ProviderDtos;
using RushHour.Domain.Enums;
using System.Xml;

namespace RushHour.Services.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _repository;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ProviderService(IProviderRepository repository, IAccountRepository accountRepository, IEmployeeRepository employeeRepository)
        {
            this._repository = repository;
            _accountRepository = accountRepository;
            _employeeRepository = employeeRepository;
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

        public async Task<GetProviderDto> GetByIdAsync(Guid requesterId, Guid id)
        {
            if (requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            GetProviderDto provider = await _repository.GetByIdAsync(id);

            await CheckProviderAdminIdAndProviderId(requesterId, provider);

            return provider;
        }

        public async Task UpdateAsync(Guid requesterId, Guid id, CreateProviderDto dto)
        {
            var provider = new GetProviderDto()
            {
                Id = id,
                Name = dto.Name,
                Website = dto.Website,
                BusinessDomain = dto.BusinessDomain,
                Phone = dto.Phone,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                WorkingDays = dto.WorkingDays,
            };

            await CheckProviderAdminIdAndProviderId(requesterId, provider);

            await _repository.UpdateAsync(id, dto);
        }

        private async Task CheckProviderAdminIdAndProviderId(Guid requesterId, GetProviderDto dto)
        {
            var currentAccount = await _accountRepository.GetByIdAsync(requesterId);

            var employees = await _employeeRepository.GetPageAsync(1, 10);

            var providerAdminEmployee = employees.Result.Where(e => e.ProviderId == dto.Id).FirstOrDefault();

            if (currentAccount.Role == Role.ProviderAdmin && providerAdminEmployee is null)
            {
                throw new UnauthorizedAccessException("Can't access another provider");
            }
        }
    }
}
