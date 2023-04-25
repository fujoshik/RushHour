using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ClientDtos;
using RushHour.Data.Extensions;

namespace RushHour.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<Client> Clients { get; }

        public ClientRepository(RushHourDbContext context)
        {
            _context = context;

            Clients = _context.Set<Client>();
        }

        public async Task<GetClientDto> CreateAsync(GetAccountDto account, CreateClientDto client)
        {
            Client entity = new()
            {
                Id = Guid.NewGuid(),
                Phone = client.Phone,
                Address = client.Address,
                AccountId = account.Id
            };

            Clients.Add(entity);

            await _context.SaveChangesAsync();

            return new GetClientDto()
            {
                Id = entity.Id,
                Phone = entity.Phone,
                Address = entity.Address,
                AccountId = entity.AccountId
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Clients.Include(e => e.Account).FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Client)} with id: {id}");
            }

            _context.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<GetClientDto>> GetPageAsync(int index, int pageSize)
        {
            return await Clients.Include(e => e.Account).Select(dto => new GetClientDto()
            {
                Id = dto.Id,
                Phone = dto.Phone,
                Address = dto.Address,
                AccountId = dto.AccountId,
                Account = new GetAccountDto()
                {
                    Id = dto.Account.Id,
                    FullName = dto.Account.FullName,
                    Email = dto.Account.Email,
                    Role = dto.Account.Role
                }
            }).PaginateAsync(index, pageSize);
        }

        public async Task<GetClientDto> GetByIdAsync(Guid id)
        {
            var entity = await Clients.Include(e => e.Account).FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Client)} with id: {id}");
            }

            return new GetClientDto()
            {
                Id = entity.Id,
                Phone = entity.Phone,
                Address = entity.Address,
                AccountId = entity.AccountId,
                Account = new GetAccountDto()
                {
                    Id = entity.Account.Id,
                    FullName = entity.Account.FullName,
                    Email = entity.Account.Email,
                    Role = entity.Account.Role
                }
            };
        }

        public async Task UpdateAsync(Guid id, UpdateClientDto dto)
        {
            var entity = await Clients.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Client)} with id: {id}");
            }

            entity.Phone = dto.Phone;
            entity.Address = dto.Address;

            await _context.SaveChangesAsync();
        }
    }
}
