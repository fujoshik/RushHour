using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Data.Extensions;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ProviderDtos;

namespace RushHour.Data.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<Provider> Providers { get; }
        private readonly IMapper _mapper;

        public ProviderRepository(RushHourDbContext context, IMapper mapper)
        {
            _context = context;
            Providers = _context.Set<Provider>();
            _mapper = mapper;
        }

        public async Task<GetProviderDto> CreateAsync(CreateProviderDto dto)
        {
            Provider entity = _mapper.Map<Provider>(dto);

            entity.Id = Guid.NewGuid();

            Providers.Add(entity);

            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetProviderDto>(entity);

            return mapped;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Providers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Provider)} with id: {id}");
            }

            _context.Remove(entity);

            await CascadeDelete(id);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<GetProviderDto>> GetPageAsync(int index, int pageSize)
        {
            var result = Providers.ConstructResult(index, pageSize);

            var mappedResult = _mapper.Map<List<GetProviderDto>>(result);

            return mappedResult.PaginateResult(index, pageSize);  
        }

        public async Task<GetProviderDto> GetByIdAsync(Guid id)
        {
            var entity = await Providers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Provider)} with id: {id}");
            }

            var mapped = _mapper.Map<GetProviderDto>(entity);

            return mapped;
        }

        public async Task UpdateAsync(Guid id, CreateProviderDto dto)
        {
            var entity = await Providers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Provider)} with id: {id}");
            }

            var mapped = _mapper.Map<Provider>(dto);
            mapped.Id = entity.Id;

            _context.Entry(entity).CurrentValues.SetValues(mapped);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        private async Task CascadeDelete(Guid id)
        {
            var employees = await _context.Set<Employee>()
                .Where(e => e.ProviderId == id)
                .ToListAsync();

            var activities = await _context.Set<Activity>()
                .Where(a => a.ProviderId == id)
                .ToListAsync();

            foreach (var employee in employees)
            {
                await _context.Set<Appointment>()
                    .Where(a => a.EmployeeId == employee.Id)
                    .ExecuteDeleteAsync();
            }

            await DeleteEmployees(employees, activities);                 
        }

        private async Task DeleteEmployees(List<Employee> employees, List<Activity> activities)
        {
            List<Account> accounts = new List<Account>();

            foreach (var employee in employees)
            {
                await _context.Set<ActivityEmployee>()
                    .Where(ae => ae.EmployeeId == employee.Id)
                    .ExecuteDeleteAsync();

                var account = await _context.Set<Account>().FindAsync(employee.AccountId);
                
                if(account is not null)
                    accounts.Add(account);

            }

            await DeleteActivities(activities);

            await _context.Set<Account>()
                .Where(a => accounts.Contains(a))
                .ExecuteDeleteAsync();

            await _context.Set<Employee>()
                .Where(e => employees.Contains(e))
                .ExecuteDeleteAsync();
        }

        private async Task DeleteActivities(List<Activity> activities)
        {
            foreach (var activity in activities)
            {
                await _context.Set<ActivityEmployee>()
                    .Where(ae => ae.ActivityId == activity.Id)
                    .ExecuteDeleteAsync();
            }

            await _context.Set<Activity>()
                .Where(a => activities.Contains(a))
                .ExecuteDeleteAsync();
        }
    }
}
