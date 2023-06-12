namespace RushHour.Domain.Abstractions.Services
{
    public interface IAccountService
    {
        byte[] GenerateSalt();
        string HashPasword(string password, byte[] salt);
        bool VerifyPassword(string password, string hash, byte[] salt);
    }
}
