using System.ComponentModel.DataAnnotations;

namespace RushHour.Data.Entities
{
    public class Activity : BaseEntity
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Duration { get; set; }

        public Guid ProviderId { get; set; }

        [Required]
        public Provider Provider { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
