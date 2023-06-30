using AutoMapper;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.AppointmentDtos;
using RushHour.Domain.Enums;

namespace RushHour.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IClientService _clientService;
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderWorkingDaysRepository _providerWorkingDaysRepo;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository appointmentRepository, 
            IAccountRepository accountRepository, IEmployeeService employeeService, 
            IClientService clientService, IActivityRepository activityRepository, IProviderRepository providerRepository, 
            IProviderWorkingDaysRepository providerWorkingDaysRepo, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _accountRepository = accountRepository;
            _employeeService = employeeService;
            _clientService = clientService;
            _activityRepository = activityRepository;
            _providerRepository = providerRepository;
            _providerWorkingDaysRepo = providerWorkingDaysRepo;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<GetAppointmentDto> CreateAppointmentAsync(Guid requesterAccountId, CreateAppointmentDto dto)
        {
            var getAppointment = _mapper.Map<GetAppointmentDto>(dto);
            getAppointment.EndDate = await CalculateEndDateOfAppointment(dto.StartDate, dto.ActivityId);

            await CheckProviderAvailability(getAppointment);

            await CheckEmployeeAvailability(getAppointment);

            var requesterAccount = await _accountRepository.GetByIdAsync(requesterAccountId);

            await CheckRequesterIdAndAppointment(requesterAccount, getAppointment);

            var appointment = await _appointmentRepository.CreateAsync(dto, getAppointment.EndDate);

            appointment.TotalPrice = await CalculateTotalPriceOfActivities(appointment.ActivityId);
            
            return appointment;
        }

        public async Task DeleteAsync(Guid requesterAccountId, Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (requesterAccountId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterAccountId));
            }

            var requesterAccount = await _accountRepository.GetByIdAsync(requesterAccountId);

            var appointment = await _appointmentRepository.GetByIdAsync(id);

            await CheckRequesterIdAndAppointment(requesterAccount, appointment);

            await CheckRequesterIdAndEmployeeId(requesterAccount, appointment);

            await _appointmentRepository.DeleteAsync(id);
        }

        public async Task<PaginatedResult<GetAppointmentDto>> GetPageAsync(int index, int pageSize, Guid requesterAccountId)
        {
            if (requesterAccountId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterAccountId));
            }

            PaginatedResult<GetAppointmentDto> appointments = new(new List<GetAppointmentDto>(), 0);

            appointments = await GetAppointmentsBasedOnRole(appointments, requesterAccountId, index, pageSize);

            foreach (var appointment in appointments.Result)
            {
                appointment.TotalPrice = await CalculateTotalPriceOfActivities(appointment.ActivityId);
            }

            return appointments;
        }

        public async Task<GetAppointmentDto> GetAppointmentByIdAsync(Guid requesterAccountId, Guid id)
        {
            if (requesterAccountId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterAccountId));
            }

            var appointment = await _appointmentRepository.GetByIdAsync(id);

            var requesterAccount = await _accountRepository.GetByIdAsync(requesterAccountId);

            await CheckRequesterIdAndAppointment(requesterAccount, appointment);

            await CheckRequesterIdAndEmployeeId(requesterAccount, appointment);

            appointment.TotalPrice = await CalculateTotalPriceOfActivities(appointment.ActivityId);

            return appointment;
        }

        public async Task UpdateAppointmentAsync(Guid id, CreateAppointmentDto dto, Guid requesterAccountId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if(appointment is null || id == default(Guid))
            {
                throw new KeyNotFoundException($"No such {typeof(GetAppointmentDto)} with id: {id}");
            }

            var newAppointmentDto = _mapper.Map<GetAppointmentDto>(dto);
            newAppointmentDto.Id = id;
            newAppointmentDto.EndDate = await CalculateEndDateOfAppointment(dto.StartDate, dto.ActivityId);

            await CheckProviderAvailability(newAppointmentDto);

            await CheckEmployeeAvailability(newAppointmentDto);

            var requesterAccount = await _accountRepository.GetByIdAsync(requesterAccountId);

            await CheckRequesterIdAndAppointment(requesterAccount, newAppointmentDto);

            await CheckRequesterIdAndEmployeeId(requesterAccount, newAppointmentDto);

            await _appointmentRepository.UpdateAsync(id, dto, newAppointmentDto.EndDate);
        }     

        private async Task<decimal> CalculateTotalPriceOfActivities(Guid activityId)
        {   
            var activity = await _activityRepository.GetByIdAsync(activityId);

            return activity.Price;
        }

        private async Task<DateTime> CalculateEndDateOfAppointment(DateTime startDate, Guid activityId)
        {
            var activity = await _activityRepository.GetByIdAsync(activityId);

            return startDate.AddMinutes(activity.Duration);
        }

        private async Task CheckProviderAvailability(GetAppointmentDto appointmentDto)
        {
            var activity = await _activityRepository.GetByIdAsync(appointmentDto.ActivityId);

            var provider = await _providerRepository.GetByIdAsync(activity.ProviderId);

            var workingDays = await _providerWorkingDaysRepo.GetAllWorkingDaysOfProviderAsync(activity.ProviderId);

            if(!workingDays.Any(d => (DayOfWeek)d.DayOfTheWeek == appointmentDto.StartDate.DayOfWeek) && 
                !workingDays.Any(d => (DayOfWeek)d.DayOfTheWeek == appointmentDto.EndDate.DayOfWeek))
            {
                throw new ArgumentOutOfRangeException(nameof(appointmentDto));
            }

            if(appointmentDto.StartDate.Hour < provider.StartTime.Hour && appointmentDto.StartDate.Minute < provider.StartTime.Minute)
            {
                throw new ArgumentOutOfRangeException(nameof(appointmentDto));
            }

            if(appointmentDto.EndDate.Hour > provider.EndTime.Hour && appointmentDto.EndDate.Minute < provider.EndTime.Minute)
            {
                throw new ArgumentOutOfRangeException(nameof(appointmentDto));
            }
        }

        private async Task CheckEmployeeAvailability(GetAppointmentDto appointmentDto)
        {
            var employee = await _employeeRepository.GetByIdAsync(appointmentDto.EmployeeId);

            var appointments = await _appointmentRepository.GetPageAsync(1, 10, Role.Employee, employee.Id);

            if (appointments.Result
                .Any(a => a.StartDate.Date == appointmentDto.StartDate.Date && a.Id != appointmentDto.Id))
            {
                if (appointments.Result
                    .Any(a => a.StartDate.Hour == appointmentDto.StartDate.Hour && a.StartDate.Minute == appointmentDto.StartDate.Minute
                    && a.Id != appointmentDto.Id))
                {
                    throw new ArgumentOutOfRangeException(nameof(appointmentDto));
                }

                if (appointments.Result
                        .Any(a => a.StartDate.Hour == appointmentDto.StartDate.Hour && a.EndDate.Minute > appointmentDto.StartDate.Minute
                        && a.Id != appointmentDto.Id))
                {
                    throw new ArgumentOutOfRangeException(nameof(appointmentDto));
                }

                if (appointments.Result
                    .Any(a => a.StartDate.Hour == appointmentDto.EndDate.Hour && a.StartDate.Minute <= appointmentDto.EndDate.Minute 
                    && a.Id != appointmentDto.Id))
                {
                    throw new ArgumentOutOfRangeException(nameof(appointmentDto));
                }
            }
        }

        private async Task CheckRequesterIdAndAppointment(AccountDto requesterAccount, GetAppointmentDto appointment)
        {
            if(requesterAccount.Role == Role.Client)
            {
                var client = await _clientService.GetClientByAccountAsync(requesterAccount.Id);

                if (appointment.ClientId != client.Id)
                {
                    throw new UnauthorizedAccessException("Can't access an appointment for another client");
                }
            }

            else if (requesterAccount.Role == Role.ProviderAdmin)
            {
                await _employeeService.GetEmployeeByIdAsync(requesterAccount.Id, appointment.EmployeeId);
            }         
        }

        private async Task CheckRequesterIdAndEmployeeId(AccountDto requesterAccount, GetAppointmentDto appointment)
        {
            if (requesterAccount.Role == Role.Employee)
            {
                var employee = await _employeeService.GetEmployeeByAccountAsync(requesterAccount.Id);

                if (appointment.EmployeeId != employee.Id)
                {
                    throw new UnauthorizedAccessException("Can't access an appointment for another employee");
                }
            }
        }

        private async Task<PaginatedResult<GetAppointmentDto>> GetAppointmentsBasedOnRole(PaginatedResult<GetAppointmentDto> appointments, Guid requesterAccountId, int index, int pageSize)
        {
            var providerResult = await _accountRepository.CheckIfAnyMatchesIdAndRole(requesterAccountId, Role.ProviderAdmin);
            var employeeResult = await _accountRepository.CheckIfAnyMatchesIdAndRole(requesterAccountId, Role.Employee);
            var clientResult = await _accountRepository.CheckIfAnyMatchesIdAndRole(requesterAccountId, Role.Client);

            if (providerResult)
            {
                var employee = await _employeeService.GetEmployeeByAccountAsync(requesterAccountId);

                return appointments = await _appointmentRepository.GetPageAsync(index, pageSize, Role.ProviderAdmin, employee.Id);
            }
            else if (employeeResult)
            {
                var employee = await _employeeService.GetEmployeeByAccountAsync(requesterAccountId);

                return appointments = await _appointmentRepository.GetPageAsync(index, pageSize, Role.Employee, employee.Id);
            }
            else if (clientResult)
            {
                var client = await _clientService.GetClientByAccountAsync(requesterAccountId);

                return appointments = await _appointmentRepository.GetPageAsync(index, pageSize, Role.Client, client.Id);
            }

            return appointments = await _appointmentRepository.GetPageAsync(index, pageSize);
        }
    }
}
