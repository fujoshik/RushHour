using Microsoft.AspNetCore.Mvc;
using RushHour.API.Configuration;
using RushHour.API.Extensions;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.ActivityDtos;
using RushHour.Domain.Enums;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/activities")]
    [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Client)]
    public class ActivityController : Controller
    {
        private readonly IActivityService _service;
        private readonly Guid requesterId;

        public ActivityController(IActivityService service, IHttpContextAccessor http)
        {
            _service = service;
            requesterId = http.GetRequesterId();
        }

        [HttpGet]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Client)]
        public async Task<ActionResult<IEnumerable<GetActivityDto>>> GetPage([FromQuery] int index, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetPageAsync(index, pageSize, requesterId));
        }

        [HttpGet("{id}")]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Client)]
        public async Task<ActionResult<GetActivityDto>> GetActivity([FromRoute] Guid id)
        {
            var activity = await _service.GetActivityByIdAsync(requesterId, id);

            if (activity is null)
            {
                return NotFound();
            }

            return activity;
        }

        [HttpPost]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin)]
        public async Task<ActionResult<GetActivityDto>> CreateActivity([FromBody] CreateActivityDto activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdActivity = await _service.CreateActivityAsync(requesterId, activity);

            return CreatedAtAction(nameof(GetActivity), new { id = createdActivity.Id }, createdActivity);
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin)]
        public async Task<IActionResult> EditActivity([FromRoute] Guid id, [FromBody] CreateActivityDto activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _service.UpdateActivityAsync(id, activity, requesterId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin)]
        public async Task<IActionResult> DeleteActivity([FromRoute] Guid id)
        {
            var activity = await _service.GetActivityByIdAsync(requesterId, id);

            if (activity is null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
