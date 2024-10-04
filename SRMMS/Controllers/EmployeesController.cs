using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public EmployeesController(SRMMSContext context)
        {
            _context = context;
        }

        [HttpGet("/api/getAllEmployees")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var employees = await _context.Employees
               .Include(e => e.EmpRole) // Include related role if needed
               .Select(e => new EmployeeDTO
               {
                   EmpId = e.EmpId,
                   EmpFirstName = e.EmpFirstName,
                   EmpLastName = e.EmpLastName,
                   EmpDob = e.EmpDob,
                   EmpPhoneNumber = e.EmpPhoneNumber,
                   EmpEmail = e.EmpEmail,
                   EmpStartDate = e.EmpStartDate,
                   EmpStatus = e.EmpStatus,
                   RoleName = e.EmpRole.RoleName // Map the role name if needed
               })
               .ToListAsync();

                if (employees == null || employees.Count == 0)
                {
                    return NotFound();
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/api/addEmployee")]
        public async Task<ActionResult<EmployeeDTO>> AddEmployee(EmployeeCreateDTO employeeDto)
        {
            // Check if the employee already exists (optional, based on unique email or other fields)
            if (await _context.Employees.AnyAsync(e => e.EmpEmail == employeeDto.EmpEmail))
            {
                return BadRequest("An employee with this email already exists.");
            }

            // Set default values for role, status, and start date
            employeeDto.RoleName = employeeDto.RoleName ?? "Staff"; // Default RoleName to "Staff" if not provided
            employeeDto.EmpStatus = employeeDto.EmpStatus ?? true; // Default EmpStatus to true if not provided
            employeeDto.EmpStartDate = employeeDto.EmpStartDate ?? DateTime.Now; // Default EmpStartDate to current time if not provided

            // Create a new Employee entity from the DTO
            var newEmployee = new Employee
            {
                EmpFirstName = employeeDto.EmpFirstName,
                EmpLastName = employeeDto.EmpLastName,
                EmpDob = (DateTime)employeeDto.EmpDob,
                EmpPhoneNumber = (int)employeeDto.EmpPhoneNumber,
                EmpEmail = employeeDto.EmpEmail,
                EmpPassword = employeeDto.EmpPassword,
                EmpStartDate = employeeDto.EmpStartDate.Value, // EmpStartDate should be a DateTime
                EmpStatus = employeeDto.EmpStatus.Value // EmpStatus should be a boolean
            };

            // Handle role linking
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == employeeDto.RoleName);
            if (role != null)
            {
                newEmployee.EmpRoleId = role.RoleId; // Assign correct role ID
                newEmployee.EmpRole = role; // Link the role entity
            }
            else
            {
                return BadRequest("Invalid role provided.");
            }

            // Add the new employee to the database
            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            // Return the newly created employee data as a DTO
            var employeeResult = new EmployeeDTO
            {
                EmpId = newEmployee.EmpId,
                EmpFirstName = newEmployee.EmpFirstName,
                EmpLastName = newEmployee.EmpLastName,
                EmpDob = newEmployee.EmpDob,
                EmpPhoneNumber = newEmployee.EmpPhoneNumber,
                EmpEmail = newEmployee.EmpEmail,
                EmpStartDate = newEmployee.EmpStartDate,
                EmpStatus = newEmployee.EmpStatus,
                RoleName = newEmployee.EmpRole?.RoleName
            };

            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmpId }, employeeResult);
        }



        [HttpGet("/api/getEmployee/{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmpRole)
                .Select(e => new EmployeeDTO
                {
                    EmpId = e.EmpId,
                    EmpFirstName = e.EmpFirstName,
                    EmpLastName = e.EmpLastName,
                    EmpDob = e.EmpDob,
                    EmpPhoneNumber = e.EmpPhoneNumber,
                    EmpEmail = e.EmpEmail,
                    EmpStartDate = e.EmpStartDate,
                    EmpStatus = e.EmpStatus,
                    RoleName = e.EmpRole.RoleName
                })
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, EmployeeUpdateDTO employeeDto)
        {
            // Fetch the existing employee by ID
            var existingEmployee = await _context.Employees
                .Include(e => e.EmpRole)
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Update fields only if they are provided (not null or default)
            if (!string.IsNullOrEmpty(employeeDto.EmpFirstName))
            {
                existingEmployee.EmpFirstName = employeeDto.EmpFirstName;
            }

            if (!string.IsNullOrEmpty(employeeDto.EmpLastName))
            {
                existingEmployee.EmpLastName = employeeDto.EmpLastName;
            }

            if (employeeDto.EmpDob.HasValue)
            {
                existingEmployee.EmpDob = employeeDto.EmpDob.Value;
            }

            if (employeeDto.EmpPhoneNumber.HasValue)
            {
                existingEmployee.EmpPhoneNumber = employeeDto.EmpPhoneNumber.Value;
            }

            if (!string.IsNullOrEmpty(employeeDto.EmpEmail))
            {
                existingEmployee.EmpEmail = employeeDto.EmpEmail;
            }

            if (!string.IsNullOrEmpty(employeeDto.EmpPassword))
            {
                existingEmployee.EmpPassword = employeeDto.EmpPassword;
            }

            if (employeeDto.EmpStartDate.HasValue)
            {
                existingEmployee.EmpStartDate = employeeDto.EmpStartDate.Value;
            }

            if (employeeDto.EmpStatus.HasValue)
            {
                existingEmployee.EmpStatus = employeeDto.EmpStatus.Value;
            }

            // Handle role update, only if a role is provided
            if (!string.IsNullOrEmpty(employeeDto.RoleName))
            {
                var roleName = employeeDto.RoleName;
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);

                if (role != null)
                {
                    existingEmployee.EmpRoleId = role.RoleId; // Assign correct role ID
                    existingEmployee.EmpRole = role; // Update role
                }
                else
                {
                    return BadRequest("Invalid role provided");
                }
            }

            _context.Entry(existingEmployee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmpId == id)).GetValueOrDefault();
        }
    }
}
