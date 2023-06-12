using RushHour.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RushHour.Data.Entities
{
    public class Account : BaseEntity
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(3)]
        public string FullName { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public string Salt { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}
