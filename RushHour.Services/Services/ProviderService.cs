using AutoMapper;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ProviderDtos;
using RushHour.Domain.DTOs.ProviderWorkingDaysDto;
using RushHour.Domain.Enums;

namespace RushHour.Services.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _repository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IProviderWorkingDaysRepository _providerWorkingDaysRepo;
        private readonly IMapper _mapper;

        public ProviderService(IProviderRepository repository, IEmployeeRepository employeeRepository, 
            IEmployeeService employeeService, IProviderWorkingDaysRepository providerWorkingDaysRepo, IMapper mapper)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
            _providerWorkingDaysRepo = providerWorkingDaysRepo;
            _mapper = mapper;
        }

        public async Task<GetProviderDto> CreateAsync(CreateProviderDto dto)
        {
            var provider = await _repository.CreateAsync(dto);

            await _providerWorkingDaysRepo.CreateProviderWithManyWorkingDaysAsync(provider.Id, ConvertStringToEnum(dto.WorkingDays));

            provider.WorkingDays = dto.WorkingDays;

            return provider;
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _providerWorkingDaysRepo.DeleteProviderWithManyWorkingDaysAsync(id);

            await _repository.DeleteAsync(id);
        }

        public async Task<PaginatedResult<GetProviderDto>> GetPageAsync(int index, int pageSize)
        {
            var providers = await _repository.GetPageAsync(index, pageSize);

            foreach (var provider in providers.Result)
            {
                var providerWorkingDays = await _providerWorkingDaysRepo.GetAllWorkingDaysOfProviderAsync(provider.Id);

                provider.WorkingDays = string.Join(", ", providerWorkingDays.Select(x => (DayOfWeek)x.DayOfTheWeek));
            }

            return providers;
        }

        public async Task<GetProviderDto> GetByIdAsync(Guid requesterId, Guid id)
        {
            if (requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            GetProviderDto provider = await _repository.GetByIdAsync(id);

            await CheckProviderAdminIdAndProviderId(requesterId, provider);

            var providerWorkingDays = await _providerWorkingDaysRepo.GetAllWorkingDaysOfProviderAsync(id);

            provider.WorkingDays = string.Join(", ", providerWorkingDays.Select(x => (DayOfWeek)x.DayOfTheWeek));

            return provider;
        }

        public async Task UpdateAsync(Guid requesterId, Guid id, CreateProviderDto dto)
        {
            if (requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            var provider = _mapper.Map<GetProviderDto>(dto);
            provider.Id = id;

            await CheckProviderAdminIdAndProviderId(requesterId, provider);

            await UpdateProviderWorkingDaysAsync(provider);

            await _repository.UpdateAsync(id, dto);
        }

        private async Task UpdateProviderWorkingDaysAsync(GetProviderDto newProviderDto)
        {
            var providerDays = await _providerWorkingDaysRepo.GetAllWorkingDaysOfProviderAsync(newProviderDto.Id);

            await DeleteOldProviderWorkingDays(providerDays, newProviderDto);

            await CreateNewProviderWorkingDays(providerDays, newProviderDto);
        }

        private async Task CreateNewProviderWorkingDays(List<ProviderWorkingDaysDto> providerDays, GetProviderDto newProviderDto)
        {
            List<DayOfWeek> daysOfTheWeekToCreate = new List<DayOfWeek>();

            foreach (var day in ConvertStringToEnum(newProviderDto.WorkingDays))
            {
                if (!providerDays.Contains(new ProviderWorkingDaysDto()
                {
                    ProviderId = newProviderDto.Id,
                    DayOfTheWeek = (int)day
                }))
                {
                    daysOfTheWeekToCreate.Add(day);
                }
            }

            await _providerWorkingDaysRepo.CreateProviderWithManyWorkingDaysAsync(newProviderDto.Id, daysOfTheWeekToCreate);
        }

        private async Task DeleteOldProviderWorkingDays(List<ProviderWorkingDaysDto> providerDays, GetProviderDto newProviderDto)
        {
            List<DayOfWeek> daysOfTheWeekToDelete = new List<DayOfWeek>();

            List<DayOfWeek> newDaysOfWeek = ConvertStringToEnum(newProviderDto.WorkingDays);

            foreach (var item in providerDays)
            {
                if (!newDaysOfWeek.Contains((DayOfWeek)item.DayOfTheWeek))
                {
                    daysOfTheWeekToDelete.Add((DayOfWeek)item.DayOfTheWeek);
                    newDaysOfWeek.Remove((DayOfWeek)item.DayOfTheWeek);
                }
            }

            await _providerWorkingDaysRepo.DeleteProviderWithManyWorkingDaysAsync(newProviderDto.Id, daysOfTheWeekToDelete);
        }

        private async Task CheckProviderAdminIdAndProviderId(Guid requesterId, GetProviderDto dto)
        {
            var currrentAccountEmployee = await _employeeService.GetEmployeeByAccountAsync(requesterId);

            if(currrentAccountEmployee is null)
            {
                return;
            }

            var employees = await _employeeRepository.GetPageAsync(1, 10);

            var providerAdminEmployee = employees.Result.Where(e => e.ProviderId == dto.Id).FirstOrDefault();

            if (currrentAccountEmployee.Account.Role == Role.ProviderAdmin && providerAdminEmployee.ProviderId != currrentAccountEmployee.ProviderId )
            {
                throw new UnauthorizedAccessException("Can't access another provider");
            }
        }

        public List<DayOfWeek> ConvertStringToEnum(string workingDays)
        {
            return workingDays
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (DayOfWeek)(int.Parse(x)))
                .ToList();
        }
    }
}
