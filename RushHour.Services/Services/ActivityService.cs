using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ActivityDtos;
using RushHour.Domain.DTOs.ActivityEmployeeDtos;
using RushHour.Domain.Enums;

namespace RushHour.Services.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IActivityEmployeeRepository _activityEmployeeRepository;

        public ActivityService(IActivityRepository activityRepository, IAccountRepository accountRepository, IEmployeeService employeeService, IActivityEmployeeRepository activityEmployeeRepository)
        {
            this._activityRepository = activityRepository;
            this._accountRepository = accountRepository;
            this._employeeService = employeeService;
            this._activityEmployeeRepository = activityEmployeeRepository;
        }

        public async Task<GetActivityDto> CreateActivityAsync(Guid requesterAccountId, CreateActivityDto dto)
        {
            var getActivity = new GetActivityDto()
            {
                Name = dto.Name,
                Price = dto.Price,
                Duration = dto.Duration,
                ProviderId = dto.ProviderId,
                EmployeeIds = dto.EmployeeIds,
            };

            await CheckProviderAdminIdAndActivityProviderId(requesterAccountId, getActivity.ProviderId);                      

            var activity = await _activityRepository.CreateAsync(dto);
            
            await _activityEmployeeRepository.CreateActivityWithManyEmployeesAsync(activity.Id, dto.EmployeeIds);

            return activity;
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _activityEmployeeRepository.DeleteActivityWithManyEmployeesAsync(id);           

            await _activityRepository.DeleteAsync(id);
        }

        public async Task<PaginatedResult<GetActivityDto>> GetPageAsync(int index, int pageSize, Guid requesterAccountId)
        {
            if (requesterAccountId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterAccountId));
            }

            var currentAccount = await _accountRepository.GetByIdAsync(requesterAccountId);

            if (currentAccount.Role == Role.ProviderAdmin)
            {
                return await _activityRepository.GetPageAsync(index, pageSize, requesterAccountId);
            }

            var activities =  await _activityRepository.GetPageAsync(index, pageSize);

            foreach (var activity in activities.Result)
            {
                var actEmps = await _activityEmployeeRepository.GetAllEmployeesOfActivityAsync(activity.Id);

                activity.EmployeeIds.AddRange(actEmps.Select(x => x.EmployeeId));
            }

            return activities;
        }

        public async Task<GetActivityDto> GetActivityByIdAsync(Guid requesterAccountId, Guid id)
        {
            if (requesterAccountId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterAccountId));
            }
            
            var activity = await _activityRepository.GetByIdAsync(id);

            await CheckProviderAdminIdAndActivityProviderId(requesterAccountId, activity.ProviderId);

            var actEmps = await _activityEmployeeRepository.GetAllEmployeesOfActivityAsync(activity.Id);

            activity.EmployeeIds.AddRange(actEmps.Select(x => x.EmployeeId));

            return activity;
        }

        public async Task UpdateActivityAsync(Guid id, CreateActivityDto dto, Guid requesterAccountId)
        {
            var newActivityDto = new GetActivityDto()
            {
                Id = id,
                Name = dto.Name,
                Price = dto.Price,
                Duration = dto.Duration,
                ProviderId = dto.ProviderId,
                EmployeeIds = dto.EmployeeIds
            };

            await CheckProviderAdminIdAndActivityProviderId(requesterAccountId, newActivityDto.ProviderId);

            await UpdateActivityEmployeesAsync(newActivityDto);

            await _activityRepository.UpdateAsync(id, dto);
        }

        private async Task UpdateActivityEmployeesAsync(GetActivityDto newActivityDto)
        {
            var actEmps = await _activityEmployeeRepository.GetAllEmployeesOfActivityAsync(newActivityDto.Id);

            await DeleteOldActivityEmployees(actEmps, newActivityDto);

            await CreateNewActivityEmployees(actEmps, newActivityDto);
        }

        private async Task CreateNewActivityEmployees(List<ActivityEmployeeDto> actEmps, GetActivityDto newActivityDto)
        {
            List<Guid> empIdsToCreate = new List<Guid>();

            foreach (var employeeId in newActivityDto.EmployeeIds)
            {
                if (!actEmps.Contains(new ActivityEmployeeDto()
                {
                    ActivityId = newActivityDto.Id,
                    EmployeeId = employeeId
                }))
                {
                    empIdsToCreate.Add(employeeId);
                }
            }

            await _activityEmployeeRepository.CreateActivityWithManyEmployeesAsync(newActivityDto.Id, empIdsToCreate);
        }

        private async Task DeleteOldActivityEmployees(List<ActivityEmployeeDto> actEmps, GetActivityDto newActivityDto)
        {
            List<Guid> empIdsToDelete = new List<Guid>();

            foreach (var item in actEmps)
            {
                if (!newActivityDto.EmployeeIds.Contains(item.EmployeeId))
                {
                    empIdsToDelete.Add(item.EmployeeId);
                    actEmps.Remove(item);
                }
            }

            await _activityEmployeeRepository.DeleteActivityWithManyEmployeesAsync(newActivityDto.Id, empIdsToDelete);
        }

        private async Task CheckProviderAdminIdAndActivityProviderId(Guid requesterAccountId, Guid dtoProviderId)
        {
            var employee = await _employeeService.GetEmployeeByAccountAsync(requesterAccountId);

            if (employee.Account.Role == Role.ProviderAdmin && dtoProviderId != employee.ProviderId)
            {
                throw new UnauthorizedAccessException("Can't access an activity with different provider");
            }
        }
    }
}
