using AutoMapper;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.EmployeeDtos;
using RushHour.Domain.Enums;

namespace RushHour.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IAccountRepository accountRepository, 
            IProviderRepository providerRepository, IAccountService accountService, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _accountRepository = accountRepository;
            _providerRepository = providerRepository;
            _accountService = accountService;
            _mapper = mapper;
        }

        public async Task<GetEmployeeDto> CreateEmployeeAsync(Guid requesterId, CreateEmployeeDto dto)
        {
            if (requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            var currentAccount = await _accountRepository.GetByIdAsync(requesterId);

            var currentAccountEmployee = await GetEmployeeByAccountAsync(requesterId);

            var getEmployee = _mapper.Map<GetEmployeeDto>(dto);

            if (currentAccount.Role == Role.ProviderAdmin)
            {
                CheckProviderAdminIdAndEmployeeId(currentAccountEmployee, getEmployee);
            }                

            var provider = await _providerRepository.GetByIdAsync(dto.ProviderId);

            if(!dto.Account.Email.EndsWith($"@{provider.BusinessDomain}.com"))
            {
                throw new ArgumentException("Employee email should contain their provider's business domain");
            }

            var accountDto = _mapper.Map<CreateAccountDto>(dto.Account);

            var createdAccount = await CreateAccountAsync(accountDto);

            var employee = await _employeeRepository.CreateAsync(createdAccount, dto);

            return employee;
        }

        private async Task<GetAccountDto> CreateAccountAsync(CreateAccountDto dto)
        {
            if (dto.Role == Role.Admin)
            {
                throw new ArgumentException("Can't assign an employee to be Admin!");
            }

            if (dto.Role == Role.Client)
            {
                throw new ArgumentException("Can't assign an employee to be Client!");
            }

            var salt = _accountService.GenerateSalt();

            dto.Password = _accountService.HashPasword(dto.Password, salt);

            var account = await _accountRepository.CreateAsync(dto, salt);

            return _mapper.Map<GetAccountDto>(account);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var employee = await _employeeRepository.GetByIdAsync(id); 

            var accountId = employee.AccountId;

            await _accountRepository.DeleteAsync(accountId);
        }

        public async Task<PaginatedResult<GetEmployeeDto>> GetPageAsync(int index, int pageSize, Guid requesterId)
        {
            if(requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            var currentAccount = await _accountRepository.GetByIdAsync(requesterId);

            var currentAccountEmployee = await GetEmployeeByAccountAsync(requesterId);

            if(currentAccount.Role == Role.ProviderAdmin)
            {
                return await _employeeRepository.GetPageAsync(index, pageSize, currentAccountEmployee.ProviderId);
            }

            return await _employeeRepository.GetPageAsync(index, pageSize);
        }

        public async Task<GetEmployeeDto> GetEmployeeByIdAsync(Guid requesterId, Guid id)
        {
            if (requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            var currentAccount = await _accountRepository.GetByIdAsync(requesterId);

            var currentAccountEmployee = await GetEmployeeByAccountAsync(requesterId);

            var employeeToRead = await _employeeRepository.GetByIdAsync(id);

            if (currentAccount.Role == Role.ProviderAdmin)
            {
                CheckProviderAdminIdAndEmployeeId(currentAccountEmployee, employeeToRead);
            }

            else if(currentAccount.Role == Role.Employee && employeeToRead.AccountId != requesterId)
            {
                throw new UnauthorizedAccessException("Can't access a different employee");
            }

            return employeeToRead;
        }

        private async Task<GetAccountDto> GetAccountByIdAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            return _mapper.Map<GetAccountDto>(account);
        }

        public async Task UpdateEmployeeAsync(Guid id, CreateEmployeeDto dto, Guid requesterId)
        {
            if (requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            var currentAccount = await _accountRepository.GetByIdAsync(requesterId);

            var oldEmployee = await _employeeRepository.GetByIdAsync(id);

            await UpdateAccountAsync(id, dto.Account, currentAccount);

            if(currentAccount.Role == Role.Employee)
            {
                if(dto.ProviderId != oldEmployee.ProviderId)
                {
                    throw new ArgumentException("Can't change employee Provider Id!");
                }
            }

            await _employeeRepository.UpdateAsync(id, dto);
        }

        private async Task UpdateAccountAsync(Guid employeeId, CreateAccountDto account, AccountDto currentAccount)
        {
            var getAccountDto = _mapper.Map<GetAccountDto>(currentAccount);

            if (account.Role != Role.ProviderAdmin && account.Role != Role.Employee)
            {
                throw new ArgumentException("Employee can only be of role ProviderAdmin or Employee!");
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (currentAccount.Role == Role.Admin)
            {
                await AdminUpdateAccountAsync(employeeId, account);
            }

            else if(currentAccount.Role == Role.ProviderAdmin)
            {
                await ProviderAdminUpdateAccountAsync(employee, account, getAccountDto, employee.AccountId);
            }

            else if(currentAccount.Role == Role.Employee)
            {
                if (employee.AccountId != currentAccount.Id)
                {
                    throw new UnauthorizedAccessException("Can't access a different employee");
                }

                await _accountRepository.UpdateAsync(employee.AccountId, account);
            }
        }

        private async Task AdminUpdateAccountAsync(Guid employeeId, CreateAccountDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            await _accountRepository.UpdateAsync(employee.AccountId, dto);
        }

        private async Task ProviderAdminUpdateAccountAsync(GetEmployeeDto employeeToChange, CreateAccountDto accountToChange, GetAccountDto currentAccount, Guid accountToChangeId)
        {
            var currentEmployee = await GetEmployeeByAccountAsync(currentAccount.Id);

            if (employeeToChange.ProviderId != currentEmployee.ProviderId)
            {
                throw new ArgumentException("Current user and employee must have the same provider!");
            }

            await _accountRepository.UpdateAsync(accountToChangeId, accountToChange);
        }

        public async Task<GetEmployeeDto> GetEmployeeByAccountAsync(Guid id)
        {
            var employees = await _employeeRepository.GetPageAsync(1, 10, Guid.Empty, id);

            return employees.Result.FirstOrDefault();
        }

        private void CheckProviderAdminIdAndEmployeeId(GetEmployeeDto currentAccountEmployee, GetEmployeeDto employeeDto)
        {
            if (employeeDto.ProviderId != currentAccountEmployee.ProviderId)
            {
                throw new UnauthorizedAccessException("Can't access an employee with different provider");
            }
        }
    }
}
