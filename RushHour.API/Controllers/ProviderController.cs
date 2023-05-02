using Microsoft.AspNetCore.Mvc;
using RushHour.API.Configuration;
using RushHour.API.Extensions;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.ProviderDtos;
using RushHour.Domain.Enums;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/providers")]
    [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Client)]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _service;
        private readonly Guid requesterId;

        public ProviderController(IProviderService providerService)
        {
            _service = providerService;
            requesterId = this.GetRequesterId();
        }

        [HttpGet]
        [AuthorizeRoles(Role.Admin, Role.Client)]
        public async Task<ActionResult<IEnumerable<GetProviderDto>>> GetPage([FromQuery] int index, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetPageAsync(index, pageSize));
        }

        [HttpGet("{id}")]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin)]
        public async Task<ActionResult<GetProviderDto>> GetProvider([FromRoute] Guid id)
        {
            var provider = await _service.GetByIdAsync(requesterId, id);

            if (provider is null)
            {
                return NotFound();
            }

            return provider;
        }

        [HttpPost]
        [AuthorizeRoles(Role.Admin)]
        public async Task<ActionResult<GetProviderDto>> CreateProvider([FromBody] CreateProviderDto provider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdProvider = await _service.CreateAsync(provider);

            return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.Id }, createdProvider);
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin)]
        public async Task<IActionResult> EditProvider([FromRoute] Guid id, [FromBody] CreateProviderDto provider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _service.UpdateAsync(requesterId, id, provider);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> DeleteProvider([FromRoute] Guid id)
        {
            var provider = await _service.GetByIdAsync(requesterId, id);

            if (provider is null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
