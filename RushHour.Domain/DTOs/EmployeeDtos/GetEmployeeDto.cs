using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.ProviderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Domain.DTOs.EmployeeDtos
{
    public class GetEmployeeDto : BaseDto
    {
        public string Title { get; set; }

        public string Phone { get; set; }

        public decimal RatePerHour { get; set; }

        public DateTime HireDate { get; set; }

        public Guid ProviderId { get; set; }

        public GetProviderDto Provider { get; set; }

        public GetAccountDto Account { get; set; }
    }
}
