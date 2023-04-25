using RushHour.Domain.DTOs.AccountDtos;

namespace RushHour.Domain.DTOs.ClientDtos
{
    public class GetClientDto : BaseDto
    {
        public string Phone { get; set; }

        public string Address { get; set; }

        public Guid AccountId { get; set; }

        public GetAccountDto Account { get; set; }
    }
}
