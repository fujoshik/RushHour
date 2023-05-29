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
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IActivityEmployeeRepository _activityEmployeeRepository;

        public ActivityService(IActivityRepository activityRepository, IAccountRepository accountRepository, 
            IEmployeeService employeeService, IActivityEmployeeRepository activityEmployeeRepository, IEmployeeRepository employeeRepository)
        {
            _activityRepository = activityRepository;
            _accountRepository = accountRepository;
            _employeeService = employeeService;
            _activityEmployeeRepository = activityEmployeeRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<GetActivityDto> CreateActivityAsync(Guid requesterAccountId, CreateActivityDto dto)
        {
            await CheckEmployeesProviderIdAndActivityProviderId(dto.ProviderId, dto.EmployeeIds);

            var getActivity = new GetActivityDto()
            {
                Name = dto.Name,
                Price = dto.Price,
                Duration = dto.Duration,
                ProviderId = dto.ProviderId,
                EmployeeIds = dto.EmployeeIds,
            };

            await CheckRequesterIdAndRole(requesterAccountId, getActivity.ProviderId);                      

            var activity = await _activityRepository.CreateAsync(dto);
            
            activity.EmployeeIds = dto.EmployeeIds;

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

            PaginatedResult<GetActivityDto> activities = new(new List<GetActivityDto>(), 0);

            if (currentAccount.Role == Role.ProviderAdmin)
            {
                var currentAccountEmployee = await _employeeService.GetEmployeeByAccountAsync(requesterAccountId);

                activities = await _activityRepository.GetPageAsync(index, pageSize, currentAccountEmployee.ProviderId);
            }
            else
            {
                activities = await _activityRepository.GetPageAsync(index, pageSize);
            }

            foreach (var activity in activities.Result)
            {
                var actEmps = await _activityEmployeeRepository.GetAllEmployeesOfActivityAsync(activity.Id);

                activity.EmployeeIds = new();

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

            await CheckRequesterIdAndRole(requesterAccountId, activity.ProviderId);

            var actEmps = await _activityEmployeeRepository.GetAllEmployeesOfActivityAsync(activity.Id);

            activity.EmployeeIds = new();

            activity.EmployeeIds.AddRange(actEmps.Select(x => x.EmployeeId));

            return activity;
        }

        public async Task UpdateActivityAsync(Guid id, CreateActivityDto dto, Guid requesterAccountId)
        {
            await CheckEmployeesProviderIdAndActivityProviderId(dto.ProviderId, dto.EmployeeIds);

            var newActivityDto = new GetActivityDto()
            {
                Id = id,
                Name = dto.Name,
                Price = dto.Price,
                Duration = dto.Duration,
                ProviderId = dto.ProviderId,
                EmployeeIds = dto.EmployeeIds
            };           

            await CheckRequesterIdAndRole(requesterAccountId, newActivityDto.ProviderId);

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

        private async Task CheckRequesterIdAndRole(Guid requesterAccountId, Guid dtoProviderId)
        {
            var employee = await _employeeService.GetEmployeeByAccountAsync(requesterAccountId);

            if(employee is null)
            {
                var adminResult = await _accountRepository.CheckIfAnyMatchesIdAndRole(requesterAccountId, Role.Admin);

                var clientResult = await _accountRepository.CheckIfAnyMatchesIdAndRole(requesterAccountId, Role.Client);

                if (!adminResult && !clientResult)
                {
                    throw new UnauthorizedAccessException("No access");
                }
            }

            else if(employee.Account.Role == Role.ProviderAdmin && dtoProviderId != employee.ProviderId)
            {
                throw new UnauthorizedAccessException("Can't access an activity with different provider");
            }
        }

        private async Task CheckEmployeesProviderIdAndActivityProviderId(Guid providerId, List<Guid> employeeIds)
        {
            foreach(var id in employeeIds)
            {
                var employee = await _employeeRepository.GetByIdAsync(id);

                if(employee.ProviderId != providerId)
                {
                    throw new ArgumentException("The assigned employees and the activity must have the same provider id!");
                }
            }
        }
    }
}
