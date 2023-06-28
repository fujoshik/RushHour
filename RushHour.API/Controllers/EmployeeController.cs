using Microsoft.AspNetCore.Mvc;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.EmployeeDtos;
using RushHour.Domain.Enums;
using RushHour.API.Configuration;
using RushHour.API.Extensions;

namespace RushHour.API.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Employee)]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly Guid requesterId;

        public EmployeeController(IEmployeeService service, IHttpContextAccessor http)
        {
            _service = service;
            requesterId = http.GetRequesterId();
        }

        [HttpGet]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin)]
        public async Task<ActionResult<IEnumerable<GetEmployeeDto>>> GetPage([FromQuery] int index, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetPageAsync(index, pageSize, requesterId));
        }

        [HttpGet("{id}")]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Employee)]
        public async Task<ActionResult<GetEmployeeDto>> GetEmployee([FromRoute] Guid id)
        {
            var employee = await _service.GetEmployeeByIdAsync(requesterId, id);

            if (employee is null)
            {
                return NotFound();
            }

            return employee;
        }

        [HttpPost]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin)]
        public async Task<ActionResult<GetEmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdEmployee = await _service.CreateEmployeeAsync(requesterId, employee);

            return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.Id }, createdEmployee);
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin, Role.Employee)]
        public async Task<IActionResult> EditEmployee([FromRoute] Guid id, [FromBody] CreateEmployeeDto employee)
        {
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
        [AuthorizeRoles(Role.Admin, Role.ProviderAdmin)]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            var employee = await _service.GetEmployeeByIdAsync(requesterId, id);

            if (employee is null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
