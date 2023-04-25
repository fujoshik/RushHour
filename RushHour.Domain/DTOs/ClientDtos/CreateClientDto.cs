using RushHour.Domain.DTOs.AccountDtos;
using System.ComponentModel.DataAnnotations;

namespace RushHour.Domain.DTOs.ClientDtos
{
    public class CreateClientDto
    {
        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        public CreateAccountDto Account { get; set; }
    }
}
