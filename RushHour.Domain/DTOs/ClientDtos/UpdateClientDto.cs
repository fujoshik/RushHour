using RushHour.Domain.DTOs.AccountDtos;

namespace RushHour.Domain.DTOs.ClientDtos
{
    public class UpdateClientDto
    {
        public string Phone { get; set; }

        public string Address { get; set; }

        public UpdateAccountWithoutRole Account { get; set; }
    }
}
