using RushHour.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Domain.DTOs.AccountDtos
{
    public class CreateAccountDto
    {
        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z""'\s-]*$")]
        public string FullName { get; set; }

        [PasswordPropertyText]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Z])")]
        public string Password { get; set; }

        public Role Role { get; set; }
    }
}
