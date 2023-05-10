using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RushHour.Data.Entities
{
    [Index(nameof(Name), nameof(BusinessDomain), IsUnique = true)]
    public class Provider : BaseEntity
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        public string Website { get; set; }

        [Required]
        [MinLength(2)]
        public string BusinessDomain { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndTime { get; set; }

        [Required]
        public int WorkingDays { get; set; }

        public List<Employee> Employees { get; set; }

        public List<Activity> Activities { get; set; }
    }
}
