using Microsoft.AspNetCore.Mvc;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.EmployeeDtos;
using RushHour.Domain.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RushHour.API.Configuration;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Employee)]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEmployeeDto>>> GetPage([FromQuery] int index, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetPageAsync(index, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetEmployeeDto>> GetEmployee([FromRoute] Guid id)
        {
            var employee = await _service.GetEmployeeByIdAsync(id);

            if (employee is null)
            {
                return NotFound();
            }

            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<GetEmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdEmployee = await _service.CreateEmployeeAsync(employee);

            return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.Id }, createdEmployee);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditEmployee([FromRoute] Guid id, [FromBody] CreateEmployeeDto employee)
        {
            var requesterId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _service.UpdateEmployeeAsync(id, employee, requesterId);
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
            var employee = await _service.GetEmployeeByIdAsync(id);

            if (employee is null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
