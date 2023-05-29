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

        public AppointmentRepository(RushHourDbContext context)
        {
            _context = context;

            Appointments = _context.Set<Appointment>();
        }

        public async Task<GetAppointmentDto> CreateAsync(CreateAppointmentDto appointment, DateTime endDate)
        {
            Appointment entity = new()
            {
                Id = Guid.NewGuid(),
                StartDate = appointment.StartDate,
                EndDate = endDate,
                EmployeeId = appointment.EmployeeId,
                ClientId = appointment.ClientId
            };

            Appointments.Add(entity);

            await _context.SaveChangesAsync();

            return new GetAppointmentDto()
            {
                Id = entity.Id,
                StartDate = appointment.StartDate,
                EndDate = endDate,
                EmployeeId = appointment.EmployeeId,
                ClientId = appointment.ClientId
            };
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

            return await appointments.Select(dto => new GetAppointmentDto()
            {
                Id = dto.Id,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EmployeeId = dto.EmployeeId,
                ClientId = dto.ClientId
            }).PaginateAsync(index, pageSize);
        }

        public async Task<GetAppointmentDto> GetByIdAsync(Guid id)
        {
            var entity = await Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Appointment)} with id: {id}");
            }

            return new GetAppointmentDto()
            {
                Id = entity.Id,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                EmployeeId = entity.EmployeeId,
                ClientId = entity.ClientId
            };
        }

        public async Task UpdateAsync(Guid id, CreateAppointmentDto dto, DateTime endDate)
        {
            var entity = await Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No such {typeof(Appointment)} with id: {id}");
            }

            entity.StartDate = dto.StartDate;
            entity.EndDate = endDate;
            entity.EmployeeId = dto.EmployeeId;
            entity.ClientId = dto.ClientId;

            await _context.SaveChangesAsync();
        }
    }
}
