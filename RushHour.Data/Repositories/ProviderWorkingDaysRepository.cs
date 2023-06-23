using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RushHour.Data.Entities;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs.ProviderWorkingDaysDto;

namespace RushHour.Data.Repositories
{
    public class ProviderWorkingDaysRepository : IProviderWorkingDaysRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<ProviderWorkingDays> ProviderWorkingDays { get; }
        private readonly IMapper _mapper;

        public ProviderWorkingDaysRepository(RushHourDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            ProviderWorkingDays = _context.Set<ProviderWorkingDays>();
        }

        public async Task CreateProviderWithManyWorkingDaysAsync(Guid providerId, List<DayOfWeek> workingDays)
        {
            foreach (var day in workingDays)
            {
                ProviderWorkingDays entity = new()
                {
                    ProviderId = providerId,
                    DayOfTheWeek = day
                };

                ProviderWorkingDays.Add(entity);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProviderWithManyWorkingDaysAsync(Guid providerId, List<DayOfWeek> workingDays = null)
        {
            if (workingDays.IsNullOrEmpty())
            {
                var daysOfCurrentProvider = await GetAllWorkingDaysOfProviderAsync(providerId);

                workingDays = daysOfCurrentProvider.Select(x => (DayOfWeek)x.DayOfTheWeek).ToList();
            }

            foreach (var day in workingDays)
            {
                var entity = await ProviderWorkingDays
                .FirstOrDefaultAsync(x => x.ProviderId == providerId && x.DayOfTheWeek == day);

                if (entity is null)
                {
                    throw new KeyNotFoundException($"No such {typeof(ProviderWorkingDays)} with provider id: {providerId} and day of the weel: {day}");
                }

                _context.Remove(entity);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProviderWorkingDaysDto>> GetAllWorkingDaysOfProviderAsync(Guid providerId)
        {
            var providerWorkingDays = ProviderWorkingDays.AsQueryable();

            if (providerId != default(Guid))
            {
                providerWorkingDays = providerWorkingDays.Where(p => p.ProviderId == providerId);
            }

            return await _mapper.ProjectTo<ProviderWorkingDaysDto>(providerWorkingDays).ToListAsync();
        }
    }
}
