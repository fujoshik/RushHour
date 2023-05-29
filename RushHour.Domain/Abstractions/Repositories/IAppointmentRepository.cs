using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AppointmentDtos;
using RushHour.Domain.Enums;

namespace RushHour.Domain.Abstractions.Repositories
{
    public interface IAppointmentRepository
    {
        Task<GetAppointmentDto> CreateAsync(CreateAppointmentDto dto, DateTime endDate);
        Task UpdateAsync(Guid id, CreateAppointmentDto dto, DateTime endDate);
        Task<GetAppointmentDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<GetAppointmentDto>> GetPageAsync(int index, int pageSize, Role role = default(Role), Guid requesterId = default(Guid));
        Task DeleteAsync(Guid id);
    }
}
