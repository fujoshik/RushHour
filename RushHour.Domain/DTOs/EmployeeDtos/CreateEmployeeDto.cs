using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.ProviderDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Domain.DTOs.EmployeeDtos
{
    public class CreateEmployeeDto
    {
        [RegularExpression(@"^[A-Za-z0-9]+$")]
        public string Title { get; set; }

        [Phone]
        public string Phone { get; set; }

        public decimal RatePerHour { get; set; }

        public DateTime HireDate { get; set; }

        public Guid ProviderId { get; set; }

        public CreateAccountDto Account { get; set;}
    }
}
