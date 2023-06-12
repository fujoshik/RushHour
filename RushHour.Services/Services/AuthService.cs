using Microsoft.IdentityModel.Tokens;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RushHour.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _repository;
        private readonly IAccountService _service;
        private readonly JwtSettings _settings;

        public AuthService(IAccountRepository repository, JwtSettings settings, IAccountService service)
        {
            _repository = repository;
            _settings = settings;
            _service = service;
        }

        public async Task RegisterAsync(CreateAccountDto accountDto)
        {
            var salt = _service.GenerateSalt();

            accountDto.Password = _service.HashPasword(accountDto.Password, salt);

            await _repository.CreateAsync(accountDto, salt);
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var users = await _repository.GetUsersByEmail(email);

            if (users == null || users.Count == 0)
            {
                return null;
            }

            var user = users.FirstOrDefault();

            if (!_service.VerifyPassword(password, user.Password, Convert.FromBase64String(user.Salt)))
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return token;
        }

        private IEnumerable<Claim> GetClaims(AccountDto dto)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, dto.Id.ToString()),
                new Claim(ClaimTypes.Role, dto.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, dto.Email),
                new Claim(JwtRegisteredClaimNames.NameId, dto.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
        }

        private string GenerateJwtToken(AccountDto dto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: GetClaims(dto),
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes),
                signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }
    }
}
