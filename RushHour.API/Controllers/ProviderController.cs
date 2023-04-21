using Microsoft.AspNetCore.Mvc;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.ProviderDtos;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/providers")]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _service;

        public ProviderController(IProviderService providerService)
        {
            _service = providerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetProviderDto>>> GetPage([FromQuery] int index, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetPageAsync(index, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProviderDto>> GetProvider([FromRoute] Guid id)
        {
            var provider = await _service.GetByIdAsync(id);

            if (provider is null)
            {
                return NotFound();
            }

            return provider;
        }

        [HttpPost]
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
        public async Task<IActionResult> EditProvider([FromRoute] Guid id, [FromBody] CreateProviderDto provider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _service.UpdateAsync(id, provider);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvider([FromRoute] Guid id)
        {
            var provider = await _service.GetByIdAsync(id);

            if (provider is null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
