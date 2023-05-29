namespace RushHour.Domain.DTOs.AppointmentDtos
{
    public class CreateAppointmentDto
    {
        public DateTime StartDate { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid ClientId { get; set; }

        public Guid ActivityId { get; set; }
    }
}
