using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Data.Extensions;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ProviderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Data.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<Provider> Providers { get; }

        public ProviderRepository(RushHourDbContext context)
        {
            _context = context;

            Providers = _context.Set<Provider>();
        }

        public async Task<GetProviderDto> CreateAsync(CreateProviderDto dto)
        {
            Provider entity = new()
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Website = dto.Website,
                BusinessDomain = dto.BusinessDomain,
                Phone = dto.Phone,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                WorkingDays = dto.WorkingDays
            };            

            Providers.Add(entity);

            await _context.SaveChangesAsync();

            return new GetProviderDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Website = entity.Website,
                BusinessDomain = entity.BusinessDomain,
                Phone = entity.Phone,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                WorkingDays = entity.WorkingDays
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Providers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Provider)} with id: {id}");
            }

            _context.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<GetProviderDto>> GetAllAsync(int index, int pageSize)
        {
            return await Providers.Select(dto => new GetProviderDto()
            {
                Id = dto.Id,
                Name = dto.Name,
                Website = dto.Website,
                BusinessDomain = dto.BusinessDomain,
                Phone = dto.Phone,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                WorkingDays = dto.WorkingDays
            }).PaginateAsync(index, pageSize);        
        }

        public async Task<GetProviderDto> GetByIdAsync(Guid id)
        {
            var entity = await Providers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Provider)} with id: {id}");
            }          

            return new GetProviderDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Website = entity.Website,
                BusinessDomain = entity.BusinessDomain,
                Phone = entity.Phone,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                WorkingDays = entity.WorkingDays
            };
        }

        public async Task UpdateAsync(GetProviderDto dto)
        {
            var entity = await Providers.FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Provider)} with id: {dto.Id}");
            }

            entity.Name = dto.Name;
            entity.Website = dto.Website;
            entity.BusinessDomain = dto.BusinessDomain;
            entity.Phone = dto.Phone;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.WorkingDays = dto.WorkingDays;

            await _context.SaveChangesAsync();
        }
    }
}
