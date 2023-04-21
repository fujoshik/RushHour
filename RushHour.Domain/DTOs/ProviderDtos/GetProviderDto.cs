using RushHour.Domain.DTOs.EmployeeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Domain.DTOs.ProviderDtos
{
    public class GetProviderDto : BaseDto
    {
        public string Name { get; set; }

        public string Website { get; set; }

        public string BusinessDomain { get; set; }

        public string Phone { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int WorkingDays { get; set; }
    }
}
