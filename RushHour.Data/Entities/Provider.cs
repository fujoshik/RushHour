using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Data.Entities
{
    [Table("Providers")]
    [Index(nameof(Name), nameof(Domain), IsUnique = true)]
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
    }
}
