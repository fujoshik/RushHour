using RushHour.Domain.DTOs.EmployeeDtos;
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

        public string BusinessDomain { get; set; }

        public string Phone { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string WorkingDays { get; set; }
    }
}
