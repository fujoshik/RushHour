using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Data.Extensions;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.EmployeeDtos;

namespace RushHour.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<Employee> Employees { get; }

        public EmployeeRepository(RushHourDbContext context)
        {
            _context = context;

            Employees = _context.Set<Employee>();
        }

        public async Task<GetEmployeeDto> CreateAsync(GetAccountDto account, CreateEmployeeDto employee)
        {
            Employee entity = new()
            {
                Id = Guid.NewGuid(),
                Title = employee.Title,
                Phone = employee.Phone,
                RatePerHour = employee.RatePerHour,
                HireDate = employee.HireDate,
                ProviderId = employee.ProviderId,
                AccountId = account.Id
            };

            Employees.Add(entity);

            await _context.SaveChangesAsync();

            return new GetEmployeeDto()
            {
                Id = entity.Id,
                Title = entity.Title,
                Phone = entity.Phone,
                RatePerHour = entity.RatePerHour,
                HireDate = entity.HireDate,
                ProviderId = entity.ProviderId,
                AccountId = entity.AccountId
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Employees.Include(e => e.Account).FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Employee)} with id: {id}");
            }

            _context.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<GetEmployeeDto>> GetPageAsync(int index, int pageSize)
        {
            return await Employees.Include(e => e.Account).Select(dto => new GetEmployeeDto()
            {
                Id = dto.Id,
                Title = dto.Title,
                Phone = dto.Phone,
                RatePerHour = dto.RatePerHour,
                HireDate = dto.HireDate,
                ProviderId = dto.ProviderId,
                Account = new GetAccountDto()
                {
                    Id = dto.Account.Id,
                    FullName = dto.Account.FullName,
                    Email = dto.Account.Email,
                    Role = dto.Account.Role
                }
            }).PaginateAsync(index, pageSize);
        }

        public async Task<GetEmployeeDto> GetByIdAsync(Guid id)
        {
            var entity = await Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Employee)} with id: {id}");
            }

            return new GetEmployeeDto()
            {
                Id = entity.Id,
                Title = entity.Title,
                Phone = entity.Phone,
                RatePerHour = entity.RatePerHour,
                HireDate = entity.HireDate,
                ProviderId = entity.ProviderId,
                AccountId = entity.AccountId
            };
        }

        public async Task UpdateAsync(Guid id, CreateEmployeeDto dto)
        {
            var entity = await Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Employee)} with id: {id}");
            }

            entity.Title = dto.Title;
            entity.Phone = dto.Phone;
            entity.RatePerHour = dto.RatePerHour;
            entity.HireDate = dto.HireDate;
            entity.ProviderId = dto.ProviderId;

            await _context.SaveChangesAsync();
        }
    }
}
