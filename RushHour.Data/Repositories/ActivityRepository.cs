using AutoMapper;
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
        private readonly IMapper _mapper;
        
        public ActivityRepository(RushHourDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            Activites = _context.Set<Activity>();
        }

        public async Task<GetActivityDto> CreateAsync(CreateActivityDto activity)
        {
            Activity entity = _mapper.Map<Activity>(activity);
            entity.Id = Guid.NewGuid();

            Activites.Add(entity);

            await _context.SaveChangesAsync();
            
            var mapped = _mapper.Map<GetActivityDto>(entity);
            
            return mapped;
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
            
            var mapped = _mapper.ProjectTo<GetActivityDto>(activities);
            
            return await mapped.PaginateAsync(index, pageSize);
        }

        public async Task<GetActivityDto> GetByIdAsync(Guid id)
        {
            var entity = await Activites.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Activity)} with id: {id}");
            }
            
            return _mapper.Map<GetActivityDto>(entity);
        }

        public async Task UpdateAsync(Guid id, CreateActivityDto dto)
        {
            var entity = await Activites.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Activity)} with id: {id}");
            }
            
            var mapped = _mapper.Map<Activity>(dto);
            mapped.Id = entity.Id;
            
            _context.Entry(entity).CurrentValues.SetValues(mapped);
            
            _context.Entry(entity).State = EntityState.Modified;
            
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
