using RushHour.Domain.CustomAttributes;
using RushHour.Domain.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RushHour.Domain.DTOs.AccountDtos
{
    public class CreateAccountDto
    {
        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z""'\s-]*$")]
        public string FullName { get; set; }

        [PasswordPropertyText]
        [Password]
        public string Password { get; set; }

        public Role Role { get; set; }
    }
}
