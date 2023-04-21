﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushHour.API.Configuration;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.Enums;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AuthorizeRoles(Role.Admin | Role.ProviderAdmin | Role.Employee)]
    public class AuthController : Controller
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(AuthDto authDto)
        {
            var token = await _service.LoginAsync(authDto.Email, authDto.Password);

            if (token is null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}
