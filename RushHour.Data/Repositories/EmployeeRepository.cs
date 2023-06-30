using AutoMapper;
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
        private readonly IMapper _mapper;

        public EmployeeRepository(RushHourDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            Employees = _context.Set<Employee>();
        }

        public async Task<GetEmployeeDto> CreateAsync(GetAccountDto account, CreateEmployeeDto employee)
        {
            Employee entity = _mapper.Map<Employee>(employee);

            entity.Id = Guid.NewGuid();
            entity.AccountId = account.Id;
            entity.Account = null;

            Employees.Add(entity);

            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetEmployeeDto>(entity);
            mapped.Account = account;

            return mapped;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Employees
                .Include(e => e.Account)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Employee)} with id: {id}");
            }

            _context.Remove(entity);

            await CascadeDelete(id);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<GetEmployeeDto>> GetPageAsync(
            int index, int pageSize, Guid requesterProviderId = default(Guid), Guid accountId = default(Guid))
        {
            var employees = Employees.Include(e => e.Account).AsQueryable();

            if (requesterProviderId != Guid.Empty)
            {
                employees = employees.Where(e => e.ProviderId == requesterProviderId);               
            }

            if(accountId != Guid.Empty)
            {
                employees = employees.Where(e => e.AccountId == accountId);
            }

            var mapped = _mapper.ProjectTo<GetEmployeeDto>(employees);

            return await mapped.PaginateAsync(index, pageSize);
        }

        public async Task<GetEmployeeDto> GetByIdAsync(Guid id)
        {
            var entity = await Employees
                .Include(e => e.Account)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Employee)} with id: {id}");
            }

            return _mapper.Map<GetEmployeeDto>(entity);
        }

        public async Task UpdateAsync(Guid id, CreateEmployeeDto dto)
        {
            var entity = await Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Employee)} with id: {id}");
            }

            var mapped = _mapper.Map<Employee>(dto);
            mapped.Id = entity.Id;
            mapped.AccountId = entity.AccountId;
            
            _context.Entry(entity).CurrentValues.SetValues(mapped);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        private async Task CascadeDelete(Guid id)
        {
            var employee = await _context.Set<Employee>().FindAsync(id);

            await _context.Set<Account>()
                .Where(a => a.Id == employee.AccountId)
                .ExecuteDeleteAsync();

            var appointments = await _context.Set<Appointment>()
                .Where(a => a.EmployeeId == id)
                .ToListAsync();

            var actEmps = await _context.Set<ActivityEmployee>()
                .Where(ae => ae.EmployeeId == id)
                .ToListAsync();

            foreach (var actEmp in actEmps)
            {
                var employeeToRemove = await _context.Set<Employee>().FindAsync(actEmp.EmployeeId);

                var activity = await _context.Set<Activity>().FindAsync(actEmp.ActivityId);

                activity.Employees.Remove(employeeToRemove);
            }

            await _context.Set<Appointment>()
                .Where(a => appointments.Contains(a))
                .ExecuteDeleteAsync();

            await _context.Set<ActivityEmployee>()
                .Where(a => actEmps.Contains(a))
                .ExecuteDeleteAsync();
        }
    }
}
