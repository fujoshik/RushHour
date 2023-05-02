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

        public EmployeeService(IEmployeeRepository employeeRepository, IAccountRepository accountRepository, IProviderRepository providerRepository)
        {
            this._employeeRepository = employeeRepository;
            this._accountRepository = accountRepository;
            this._providerRepository = providerRepository;
        }

        public async Task<GetEmployeeDto> CreateEmployeeAsync(Guid requesterId, CreateEmployeeDto dto)
        {
            if (requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            var getEmployee = new GetEmployeeDto()
            {
                Title = dto.Title,
                Phone = dto.Phone,
                RatePerHour = dto.RatePerHour,
                HireDate = dto.HireDate,
                ProviderId = dto.ProviderId
            };

            await CheckProviderAdminIdAndEmployeeId(requesterId, getEmployee);

            var provider = await _providerRepository.GetByIdAsync(dto.ProviderId);

            if(!dto.Account.Email.EndsWith($"@{provider.BusinessDomain}.com"))
            {
                throw new ArgumentException("Employee email should contain their provider's business domain");
            }           

            var createdAccount = await CreateAccountAsync(new CreateAccountDto()
            {
                Email = dto.Account.Email,
                FullName = dto.Account.FullName,
                Password = dto.Account.Password,
                Role = dto.Account.Role
            });

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

            dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var account = await _accountRepository.CreateAsync(dto);

            return new GetAccountDto()
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role
            };
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

            if(currentAccount.Role == Role.ProviderAdmin)
            {
                return await _employeeRepository.GetPageAsync(index, pageSize,requesterId);
            }

            return await _employeeRepository.GetPageAsync(index, pageSize);
        }

        public async Task<GetEmployeeDto> GetEmployeeByIdAsync(Guid requesterId, Guid id)
        {
            if (requesterId == default(Guid))
            {
                throw new ArgumentNullException(nameof(requesterId));
            }

            var employee = await _employeeRepository.GetByIdAsync(id);

            await CheckProviderAdminIdAndEmployeeId(requesterId, employee);

            if (!await CheckEmployeeId(requesterId))
            {
                throw new UnauthorizedAccessException("Can't access a different employee");
            }

            return employee;
        }

        private async Task<GetAccountDto> GetAccountByIdAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            return new GetAccountDto()
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role
            };
        }

        public async Task UpdateEmployeeAsync(Guid id, CreateEmployeeDto dto, Guid requesterId)
        {
            await UpdateAccountAsync(id, dto.Account, requesterId);

            await _employeeRepository.UpdateAsync(id, dto);
        }

        private async Task UpdateAccountAsync(Guid employeeId, CreateAccountDto account, Guid requesterId)
        {
            var currentAccount = await _accountRepository.GetByIdAsync(requesterId);

            var getAccountDto = new GetAccountDto()
            {
                Id = currentAccount.Id,
                Email = currentAccount.Email,
                FullName = currentAccount.FullName,
                Role = currentAccount.Role
            };

            if (account.Role != Role.ProviderAdmin || account.Role != Role.Employee)
            {
                throw new ArgumentException("Employee can only be of role ProviderAdmin or Employee!");
            }

            if (currentAccount.Role == Role.Admin)
            {
                await AdminUpdateAccountAsync(employeeId, account);
            }

            else if(currentAccount.Role == Role.ProviderAdmin)
            {
                await ProviderAdminUpdateAccountAsync(employeeId, account, getAccountDto);
            }

            else if(currentAccount.Role == Role.Employee)
            {
                var employee = await _employeeRepository.GetByIdAsync(employeeId);

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

        private async Task ProviderAdminUpdateAccountAsync(Guid accountToChangeId, CreateAccountDto accountToChange, GetAccountDto currentAccount)
        {
            var employeeToChange = await GetEmployeeByAccountAsync(accountToChangeId);

            var currentEmployee = await GetEmployeeByAccountAsync(currentAccount.Id);

            if (employeeToChange.ProviderId != currentEmployee.ProviderId)
            {
                throw new ArgumentException("Current user and employee must have the same provider!");
            }

            await _accountRepository.UpdateAsync(accountToChangeId, accountToChange);
        }

        private async Task<GetEmployeeDto> GetEmployeeByAccountAsync(Guid id)
        {
            var employees = await _employeeRepository.GetPageAsync(1, 10);

            return employees.Result.Where(e => e.Account.Id == id).FirstOrDefault();
        }

        private async Task CheckProviderAdminIdAndEmployeeId(Guid requesterId, GetEmployeeDto dto)
        {
            var currentAccount = await _accountRepository.GetByIdAsync(requesterId);

            if (currentAccount.Role == Role.ProviderAdmin && dto.ProviderId != currentAccount.Id)
            {
                throw new UnauthorizedAccessException("Can't access an employee with different provider");
            }
        }

        private async Task<bool> CheckEmployeeId(Guid requesterId)
        {
            return await _accountRepository.CheckIfAnyMatchesIdAndRole(requesterId, Role.Employee);
        }
    }
}
