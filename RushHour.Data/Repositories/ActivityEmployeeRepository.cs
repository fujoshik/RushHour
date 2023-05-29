using Microsoft.EntityFrameworkCore;
using RushHour.Domain.DTOs;
using RushHour.Data.Entities;
using RushHour.Domain.DTOs.ActivityEmployeeDtos;
using RushHour.Data.Extensions;
using RushHour.Domain.Abstractions.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace RushHour.Data.Repositories
{
    public class ActivityEmployeeRepository : IActivityEmployeeRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<ActivityEmployee> ActivityEmployees { get; }

        public ActivityEmployeeRepository(RushHourDbContext context)
        {
            _context = context;

            ActivityEmployees = _context.Set<ActivityEmployee>();
        }

        public async Task<ActivityEmployeeDto> CreateAsync(ActivityEmployeeDto actEmp)
        {
            ActivityEmployee entity = new()
            {
                ActivityId = actEmp.ActivityId,
                EmployeeId = actEmp.EmployeeId
            };

            ActivityEmployees.Add(entity);

            await _context.SaveChangesAsync();

            return actEmp;
        }

        public async Task CreateActivityWithManyEmployeesAsync(Guid activityId, List<Guid> employeeIds)
        {
            foreach (var id in employeeIds)
            {
                ActivityEmployee entity = new()
                {
                    ActivityId = activityId,
                    EmployeeId = id
                };

                ActivityEmployees.Add(entity);               
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid activityId, Guid employeeId)
        {
            var entity = await ActivityEmployees
                .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.EmployeeId == employeeId);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(ActivityEmployee)} with activity id: {activityId} and employee id: {employeeId}");
            }

            _context.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteActivityWithManyEmployeesAsync(Guid activityId, List<Guid> employeeIds = null)
        {
            if(employeeIds.IsNullOrEmpty())
            {
                var employeesOfCurrentActivity = await GetAllEmployeesOfActivityAsync(activityId);

                employeeIds = employeesOfCurrentActivity.Select(x => x.EmployeeId).ToList();
            }

            foreach (var employeeId in employeeIds)
            {
                var entity = await ActivityEmployees
                .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.EmployeeId == employeeId);

                if (entity is null)
                {
                    throw new KeyNotFoundException($"No such {typeof(ActivityEmployee)} with activity id: {activityId} and employee id: {employeeId}");
                }

                _context.Remove(entity);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<ActivityEmployeeDto>> GetAllEmployeesOfActivityAsync(Guid activityId)
        {
            var actEmps = ActivityEmployees.AsQueryable();

            if (activityId != default(Guid))
            {
                actEmps = actEmps.Where(ae => ae.ActivityId == activityId);
            }

            return await actEmps.Select(dto => new ActivityEmployeeDto()
            {
                ActivityId = dto.ActivityId,
                EmployeeId = dto.EmployeeId
            }).ToListAsync();
        }

        public async Task<PaginatedResult<ActivityEmployeeDto>> GetPageAsync(int index, int pageSize, Guid activityId = default(Guid))
        {
            var actEmps = ActivityEmployees.AsQueryable();

            if (activityId != default(Guid))
            {
                actEmps.Where(ae => ae.ActivityId == activityId);
            }         

            return await actEmps.Select(dto => new ActivityEmployeeDto()
            {
                ActivityId = dto.ActivityId,
                EmployeeId = dto.EmployeeId
            }).PaginateAsync(index, pageSize);
        }

        public async Task<ActivityEmployeeDto> GetByActivityAndEmployeeIdAsync(Guid activityId, Guid employeeId)
        {
            var entity = await ActivityEmployees
                .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.EmployeeId == employeeId);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(ActivityEmployee)} with activity id: {activityId} and employee id: {employeeId}");
            }

            return new ActivityEmployeeDto()
            {
                ActivityId = entity.ActivityId,
                EmployeeId = entity.EmployeeId
            };
        }
    }
}
