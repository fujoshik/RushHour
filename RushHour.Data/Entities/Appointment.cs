using System.ComponentModel.DataAnnotations;

namespace RushHour.Data.Entities
{
    public class Appointment : BaseEntity
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public Guid EmployeeId { get; set; }

        [Required]
        public Employee Employee { get; set; }

        public Guid ClientId { get; set; }

        [Required]
        public Client Client { get; set; }

        public Guid ActivityId { get; set; }

        [Required]
        public Activity Activity { get; set; }
    }
}
