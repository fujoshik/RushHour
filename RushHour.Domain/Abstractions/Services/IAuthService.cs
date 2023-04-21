using RushHour.Domain.DTOs.AccountDtos;

namespace RushHour.Domain.Abstractions.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
    }
}
