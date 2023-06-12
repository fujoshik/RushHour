using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AuthDtos;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<Token>> LoginAsync(AuthDto authDto)
        {
            var token = await _service.LoginAsync(authDto.Email, authDto.Password);

            if (token is null)
            {
                return Unauthorized();
            }

            return new Token { Jwt = token };
        }
    }
}
