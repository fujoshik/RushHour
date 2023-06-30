using Microsoft.AspNetCore.Mvc;
using RushHour.API.Configuration;
using RushHour.API.Extensions;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.AppointmentDtos;
using RushHour.Domain.Enums;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Employee, Role.Client)]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _service;
        private readonly Guid requesterId;

        public AppointmentController(IAppointmentService service, IHttpContextAccessor http)
        {
            _service = service;
            requesterId = http.GetRequesterId();
        }

        [HttpGet]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Employee, Role.Client)]
        public async Task<ActionResult<IEnumerable<GetAppointmentDto>>> GetPage([FromQuery] int index, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetPageAsync(index, pageSize, requesterId));
        }

        [HttpGet("{id}")]
        [AuthorizeRoles(Role.Admin, Role.Client, Role.Employee, Role.ProviderAdmin)]
        public async Task<ActionResult<GetAppointmentDto>> GetAppointment([FromRoute] Guid id)
        {
            var activity = await _service.GetAppointmentByIdAsync(requesterId, id);

            if (activity is null)
            {
                return NotFound();
            }

            return activity;
        }

        [HttpPost]
        [AuthorizeRoles(Role.Admin, Role.Employee, Role.Client, Role.ProviderAdmin)]
        public async Task<ActionResult<GetAppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdAppointment = await _service.CreateAppointmentAsync(requesterId, appointment);

            return CreatedAtAction(nameof(GetAppointment), new { id = createdAppointment.Id }, createdAppointment);
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(Role.Admin, Role.Client, Role.Employee, Role.ProviderAdmin)]
        public async Task<IActionResult> EditAppointment([FromRoute] Guid id, [FromBody] CreateAppointmentDto appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _service.UpdateAppointmentAsync(id, appointment, requesterId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [AuthorizeRoles(Role.Admin, Role.Client, Role.Employee, Role.ProviderAdmin)]
        public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id)
        {
            var activity = await _service.GetAppointmentByIdAsync(requesterId, id);

            if (activity is null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(requesterId, id);

            return NoContent();
        }
    }
}
