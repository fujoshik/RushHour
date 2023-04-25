﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushHour.API.Configuration;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.ClientDtos;
using RushHour.Domain.Enums;
using System.Security.Claims;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [AuthorizeRoles(Role.Admin, Role.Client)]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientController(IClientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetClientDto>>> GetPage([FromQuery] int index, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetPageAsync(index, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetClientDto>> GetClient([FromRoute] Guid id)
        {
            var client = await _service.GetClientByIdAsync(id);

            if (client is null)
            {
                return NotFound();
            }

            return client;
        }

        [HttpPost]
        public async Task<ActionResult<GetClientDto>> CreateClient([FromBody] CreateClientDto client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdClient = await _service.CreateClientAsync(client);

            return CreatedAtAction(nameof(GetClient), new { id = createdClient.Id }, createdClient);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditClient([FromRoute] Guid id, [FromBody] UpdateClientDto client)
        {
            var requesterId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _service.UpdateClientAsync(id, client, requesterId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            var client = await _service.GetClientByIdAsync(id);

            if (client is null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}