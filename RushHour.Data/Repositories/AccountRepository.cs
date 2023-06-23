using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Data.Extensions;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.Enums;

namespace RushHour.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<Account> Accounts { get; }
        private readonly IMapper _mapper;

        public AccountRepository(RushHourDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            Accounts = _context.Set<Account>();
        }

        public async Task<AccountDto> CreateAsync(CreateAccountDto dto, byte[] salt)
        {
            Account entity = _mapper.Map<Account>(dto);
            entity.Id = Guid.NewGuid();
            entity.Salt = Convert.ToBase64String(salt);

            Accounts.Add(entity);

            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<AccountDto>(entity);

            return mapped;
        }

        public async Task<PaginatedResult<AccountDto>> GetPageAsync(int index, int pageSize)
        {
            var mapped = _mapper.ProjectTo<AccountDto>(Accounts.AsQueryable());

            return await mapped.PaginateAsync(index, pageSize);
        }

        public async Task<AccountDto> GetByIdAsync(Guid id)
        {
            var entity = await Accounts.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Account)} with id: {id}");
            }

            return _mapper.Map<AccountDto>(entity);
        }

        public async Task UpdateAsync(Guid id, CreateAccountDto dto)
        {
            var entity = await Accounts.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Account)} with id: {id}");
            }

            var mapped = _mapper.Map<Account>(dto);
            mapped.Id = entity.Id;
            mapped.Password = entity.Password;
            mapped.Salt = entity.Salt;

            _context.Entry(entity).CurrentValues.SetValues(mapped);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Accounts.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Account)} with id: {id}");
            }

            _context.Remove(entity);

            if(entity.Role == Role.Employee || entity.Role == Role.ProviderAdmin)
            {
                await CascadeDelete(id);
            }           

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfAnyMatchesIdAndRole(Guid id, Role role)
        {
            return await Accounts.AnyAsync(a => a.Id == id && a.Role == role);
        }

        public async Task<List<AccountDto>> GetUsersByEmail(string email)
        {
            return await _mapper
                .ProjectTo<AccountDto>(Accounts.Where(a => a.Email == email))
                .ToListAsync();
        }
        
        private async Task CascadeDelete(Guid id)
        {
            var employee = _context.Set<Employee>().FirstOrDefault(e => e.AccountId == id);
            
            var appointments = await _context.Set<Appointment>()
                .Where(a => a.EmployeeId == employee.Id)
                .ToListAsync();
            
            var actEmps = await _context.Set<ActivityEmployee>()
                .Where(ae => ae.EmployeeId == employee.Id)
                .ToListAsync();
            
            foreach (var actEmp in actEmps)
            {
                var employeeToRemove = await _context.Set<Employee>().FindAsync(actEmp.EmployeeId);
                
                var activity = await _context.Set<Activity>().FindAsync(actEmp.ActivityId);
                
                activity.Employees.Remove(employeeToRemove);
            }
            
            appointments.ForEach(a => _context.Set<Appointment>().Remove(a));
            
            actEmps.ForEach(ae => _context.Set<ActivityEmployee>().Remove(ae));
            
            _context.Set<Employee>().Remove(employee);
        }
	}
}
