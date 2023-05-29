using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

        [NotMapped]
        public List<DayOfWeek> WorkingDays { get; set; }

        public List<Employee> Employees { get; set; }

        public List<Activity> Activities { get; set; }
    }
}
