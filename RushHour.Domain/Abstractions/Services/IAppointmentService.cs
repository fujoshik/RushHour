using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AppointmentDtos;

namespace RushHour.Domain.Abstractions.Services
{
    public interface IAppointmentService
    {
        Task<GetAppointmentDto> CreateAppointmentAsync(Guid requesterAccountId, CreateAppointmentDto dto);
        Task<GetAppointmentDto> GetAppointmentByIdAsync(Guid requesterAccountId, Guid id);
        Task<PaginatedResult<GetAppointmentDto>> GetPageAsync(int index, int pageSize, Guid requesterAccountId);
        Task UpdateAppointmentAsync(Guid id, CreateAppointmentDto dto, Guid requesterAccountId);
        Task DeleteAsync(Guid requesterAccountId, Guid id);
    }
}
