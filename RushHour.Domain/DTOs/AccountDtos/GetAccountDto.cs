using RushHour.Domain.Enums;

namespace RushHour.Domain.DTOs.AccountDtos
{
    public class GetAccountDto : BaseDto
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public Role Role { get; set; }
    }
}
