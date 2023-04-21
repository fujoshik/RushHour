using Microsoft.IdentityModel.Tokens;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _repository;
        private readonly JwtSettings _settings;

        public AuthService(IAccountRepository repository, JwtSettings settings)
        {
            _repository = repository;
            _settings = settings;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var allUsers = await _repository.GetPageAsync(1, 10);
            var users = allUsers.Result.Where(u => u.Email == email).ToList();

            if (users == null || users.Count == 0)
            {
                return null;
            }

            var user = users.FirstOrDefault();

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
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
