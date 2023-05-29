using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Data.Extensions;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ActivityDtos;

namespace RushHour.Data.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<Activity> Activites { get; }

        public ActivityRepository(RushHourDbContext context)
        {
            _context = context;

            Activites = _context.Set<Activity>();
        }

        public async Task<GetActivityDto> CreateAsync(CreateActivityDto activity)
        {
            Activity entity = new()
            {
                Id = Guid.NewGuid(),
                Name = activity.Name,
                Price = activity.Price,
                Duration = activity.Duration,
                ProviderId = activity.ProviderId
            };

            Activites.Add(entity);

            await _context.SaveChangesAsync();

            return new GetActivityDto()
            {
                Id = entity.Id,
                Name = activity.Name,
                Price = activity.Price,
                Duration = activity.Duration,
                ProviderId = activity.ProviderId
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Activites.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Activity)} with id: {id}");
            }

            _context.Remove(entity);

            await CascadeDelete(id);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<GetActivityDto>> GetPageAsync(int index, int pageSize, Guid requesterProviderId = default(Guid))
        {
            var activities = Activites.AsQueryable();

            if (requesterProviderId != Guid.Empty)
            {
                activities = activities.Where(e => e.ProviderId == requesterProviderId);
            }

            return await activities.Select(dto => new GetActivityDto()
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                Duration = dto.Duration,
                ProviderId = dto.ProviderId              
            }).PaginateAsync(index, pageSize);
        }

        public async Task<GetActivityDto> GetByIdAsync(Guid id)
        {
            var entity = await Activites.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Activity)} with id: {id}");
            }

            return new GetActivityDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                Duration = entity.Duration,
                ProviderId = entity.ProviderId
            };
        }

        public async Task UpdateAsync(Guid id, CreateActivityDto dto)
        {
            var entity = await Activites.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Activity)} with id: {id}");
            }

            entity.Name = dto.Name;
            entity.Price = dto.Price;
            entity.Duration = dto.Duration;
            entity.ProviderId = dto.ProviderId;

            await _context.SaveChangesAsync();
        }

        private async Task CascadeDelete(Guid id)
        {
            var appointments = await _context.Set<Appointment>()
                .Where(a => a.ActivityId == id)
                .ToListAsync();

            appointments.ForEach(a => _context.Set<Appointment>().Remove(a));

            var actEmps = await _context.Set<ActivityEmployee>()
                  .Where(ae => ae.ActivityId == id)
                  .ToListAsync();

            actEmps.ForEach(ae => _context.Set<ActivityEmployee>().Remove(ae));
        }
    }
}
