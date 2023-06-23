using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.ClientDtos;
using RushHour.Data.Extensions;
using AutoMapper;

namespace RushHour.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<Client> Clients { get; }
        private readonly IMapper _mapper;

        public ClientRepository(RushHourDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            Clients = _context.Set<Client>();
        }

        public async Task<GetClientDto> CreateAsync(GetAccountDto account, CreateClientDto client)
        {
            Client entity = _mapper.Map<Client>(client);

            entity.Id = Guid.NewGuid();
            entity.AccountId = account.Id;
            entity.Account = null;

            Clients.Add(entity);

            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetClientDto>(entity);
            mapped.Account = account;

            return mapped;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Clients.Include(e => e.Account).FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Client)} with id: {id}");
            }

            _context.Remove(entity);

            await CascadeDelete(id);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<GetClientDto>> GetPageAsync(int index, int pageSize, Guid requesterClientId = default(Guid), Guid accountId = default(Guid))
        {
            var clients = Clients.Include(e => e.Account).AsQueryable();

            if (requesterClientId != Guid.Empty)
            {
                clients = clients.Where(e => e.AccountId == requesterClientId);
            }

            if (accountId != Guid.Empty)
            {
                clients = clients.Where(e => e.AccountId == accountId);
            }

            var mapped = _mapper.ProjectTo<GetClientDto>(clients);

            return await mapped.PaginateAsync(index, pageSize);
        }

        public async Task<GetClientDto> GetByIdAsync(Guid id)
        {
            var entity = await Clients.Include(e => e.Account).FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Client)} with id: {id}");
            }

            return _mapper.Map<GetClientDto>(entity);
        }

        public async Task UpdateAsync(Guid id, UpdateClientDto dto)
        {
            var entity = await Clients.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Client)} with id: {id}");
            }

            var mapped = _mapper.Map<Client>(dto);
            mapped.Id = entity.Id;
            mapped.AccountId = entity.AccountId;

            _context.Entry(entity).CurrentValues.SetValues(mapped);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        private async Task CascadeDelete(Guid id)
        {
            var client = await _context.Set<Client>().FindAsync(id);

            var account = await _context.Set<Account>().FindAsync(client.AccountId);
            _context.Set<Account>().Remove(account);

            var appointments = await _context.Set<Appointment>()
                .Where(a => a.ClientId == id)
                .ToListAsync();

            appointments.ForEach(a => _context.Set<Appointment>().Remove(a)); ;
        }
    }
}
