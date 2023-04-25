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

        public AccountRepository(RushHourDbContext context)
        {
            _context = context;

            Accounts = _context.Set<Account>();
        }

        public async Task<AccountDto> CreateAsync(CreateAccountDto dto)
        {
            Account entity = new()
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FullName = dto.FullName,
                Password = dto.Password,
                Role = dto.Role
            };

            Accounts.Add(entity);

            await _context.SaveChangesAsync();

            return new AccountDto()
            {
                Id = entity.Id,
                Email = entity.Email,
                FullName = entity.FullName,
                Password = entity.Password,
                Role = entity.Role
            };
        }

        public async Task<PaginatedResult<AccountDto>> GetPageAsync(int index, int pageSize)
        {
            return await Accounts.Select(dto => new AccountDto()
            {
                Id = dto.Id,
                Email = dto.Email,
                FullName = dto.FullName,
                Password = dto.Password,
                Role = dto.Role
            }).PaginateAsync(index, pageSize);
        }

        public async Task<AccountDto> GetByIdAsync(Guid id)
        {
            var entity = await Accounts.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Account)} with id: {id}");
            }

            return new AccountDto()
            {
                Id = entity.Id,
                Email = entity.Email,
                FullName = entity.FullName,
                Password = entity.Password,
                Role = entity.Role
            };
        }

        public async Task UpdateAsync(Guid id, CreateAccountDto dto)
        {
            var entity = await Accounts.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Account)} with id: {id}");
            }

            entity.Email = dto.Email;
            entity.FullName = dto.FullName;
            entity.Role = dto.Role;

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

            await _context.SaveChangesAsync();
        }
    }
}
