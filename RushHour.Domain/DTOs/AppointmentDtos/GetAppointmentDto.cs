namespace RushHour.Domain.DTOs.AppointmentDtos
{
    public class GetAppointmentDto : BaseDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TotalPrice { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid ClientId { get; set; }

        public Guid ActivityId { get; set; }
    }
}
