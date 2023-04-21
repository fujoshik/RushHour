using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace RushHour.Data.Entities
{
    public class Employee : BaseEntity
    {
        [Required]
        [MinLength(2)]
        public string Title { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [Min(0)]
        public decimal RatePerHour { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        public Guid ProviderId { get; set; }

        [Required]
        public Provider Provider { get; set; }

        public Guid AccountId { get; set; }

        [Required]
        public Account Account { get; set; }
    }
}
