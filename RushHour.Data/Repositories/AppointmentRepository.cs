using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RushHour.Data.Entities;
using RushHour.Data.Extensions;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AppointmentDtos;
using RushHour.Domain.Enums;

namespace RushHour.Data.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        protected RushHourDbContext _context;
        protected DbSet<Appointment> Appointments { get; }
        private readonly IMapper _mapper;

        public AppointmentRepository(RushHourDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            Appointments = _context.Set<Appointment>();
        }

        public async Task<GetAppointmentDto> CreateAsync(CreateAppointmentDto appointment, DateTime endDate)
        {
            Appointment entity = _mapper.Map<Appointment>(appointment);
            entity.Id = Guid.NewGuid();
            entity.EndDate = endDate;

            Appointments.Add(entity);

            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetAppointmentDto>(entity);
            
            return mapped;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Appointment)} with id: {id}");
            }

            _context.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<GetAppointmentDto>> GetPageAsync(int index, int pageSize,
                                                                           Role role = default(Role),
                                                                           Guid requesterId = default(Guid))
        {
            var appointments = Appointments.AsQueryable();

            if(role == Role.ProviderAdmin && requesterId != Guid.Empty)
            {
                var providerAdminEmployee = _context.Set<Employee>().Find(requesterId);

                appointments = Appointments
                    .Include(a => a.Employee)
                    .Where(a => a.Employee.ProviderId == providerAdminEmployee.ProviderId);
            }

            else if(role == Role.Employee && requesterId != Guid.Empty)
            {
                appointments = Appointments.Where(a => a.EmployeeId == requesterId);
            }

            else if(role == Role.Client && requesterId != Guid.Empty)
            {
                appointments = Appointments.Where(a => a.ClientId == requesterId);
            }
            
            var mapped = _mapper.ProjectTo<GetAppointmentDto>(appointments);
            
            return await mapped.PaginateAsync(index, pageSize);
        }

        public async Task<GetAppointmentDto> GetByIdAsync(Guid id)
        {
            var entity = await Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Appointment)} with id: {id}");
            }

            return _mapper.Map<GetAppointmentDto>(entity);
        }

        public async Task UpdateAsync(Guid id, CreateAppointmentDto dto, DateTime endDate)
        {
            var entity = await Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Appointment)} with id: {id}");
            }

            var mapped = _mapper.Map<Appointment>(dto);
            mapped.Id = entity.Id;
            mapped.EndDate = endDate;

            _context.Entry(entity).CurrentValues.SetValues(mapped);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}
