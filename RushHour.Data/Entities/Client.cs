using System.ComponentModel.DataAnnotations;

namespace RushHour.Data.Entities
{
    public class Client : BaseEntity
    {
        [Required]
        public string Phone { get; set; }

        [Required]
        [MinLength(3)]
        public string Address { get; set; }

        public Guid AccountId { get; set; }

        [Required]
        public Account Account { get; set; }
    }
}
