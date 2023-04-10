using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Domain.DTOs.ProviderDtos
{
    public class CreateProviderDto
    {
        public string Name { get; set; }

        [Url]
        public string Website { get; set; }

        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z0-9""'\s-]*$", ErrorMessage = "Letters and numbers only")]
        public string BusinessDomain { get; set; }

        [Phone]
        public string Phone { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int WorkingDays { get; set; }
    }
}
