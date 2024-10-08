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
               .Include(e => e.EmpRole) 
               .Select(e => new EmployeeDTO
               {
                   EmpId = e.EmpId,
                   EmpFirstName = e.EmpFirstName,
                   EmpLastName = e.EmpLastName,
                   EmpDob = e.EmpDob.Value.ToString("yyyy-MM-dd"),
                   EmpGender = e.EmpGender ? "Male" : "Female" ,
                   EmpAddress = e.EmpAddress,
                   EmpPhoneNumber = e.EmpPhoneNumber,
                   EmpEmail = e.EmpEmail,
                   EmpStartDate = e.EmpStartDate.ToString("yyyy-MM-dd"),
                   EmpStatus = e.EmpStatus,
                   EmpRole = new RoleDTO
                   {
                       RoleId = e.EmpRole.RoleId,
                       RoleName = e.EmpRole.RoleName,
                       Description = e.EmpRole.Description
                   }
               })
               .ToListAsync();

                if (employees == null  || employees.Count == 0)
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
            
            if (await _context.Employees.AnyAsync(e => e.EmpEmail == employeeDto.EmpEmail))
            {
                return BadRequest("An employee with this email already exists.");
            }

           
            employeeDto.RoleId = employeeDto.RoleId; 
            employeeDto.EmpStatus = employeeDto.EmpStatus ?? true; 
            employeeDto.EmpStartDate = employeeDto.EmpStartDate ?? DateTime.Now; 

            
            var newEmployee = new Employee
            {
                EmpFirstName = employeeDto.EmpFirstName,
                EmpLastName = employeeDto.EmpLastName,
                EmpDob = (DateTime)employeeDto.EmpDob,
                EmpPhoneNumber = (int)employeeDto.EmpPhoneNumber,
                EmpGender = (bool)employeeDto.EmpGender,
                EmpEmail = employeeDto.EmpEmail,
                EmpPassword = employeeDto.EmpPassword,
                EmpAddress = employeeDto.EmpAddress,
                EmpStartDate = employeeDto.EmpStartDate.Value, 
                EmpStatus = employeeDto.EmpStatus.Value 
            };

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == employeeDto.RoleId);
            if (role != null)
            {
                newEmployee.EmpRoleId = role.RoleId; 
                newEmployee.EmpRole = role; 
            }
            else
            {
                return BadRequest("Invalid role provided.");
            }

            
            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

          
            var employeeResult = new EmployeeDTO
            {
                EmpId = newEmployee.EmpId,
                EmpFirstName = newEmployee.EmpFirstName,
                EmpLastName = newEmployee.EmpLastName,
                EmpDob = newEmployee.EmpDob.Value.ToString("yyyy-MM-dd"),
                EmpPhoneNumber = newEmployee.EmpPhoneNumber,
                EmpEmail = newEmployee.EmpEmail,
                EmpStartDate = newEmployee.EmpStartDate.ToString("yyyy-MM-dd"),
                EmpStatus = newEmployee.EmpStatus,
                EmpRoleId = newEmployee.EmpRoleId,
            };

            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmpId }, employeeResult);
        }



        [HttpGet("/api/getEmployeeByID/{id}")]
        public async Task<ActionResult<EmployeeCreateDTO>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmpRole)
                .Select(e => new EmployeeCreateDTO
                {
                    EmpId = e.EmpId,
                    EmpFirstName = e.EmpFirstName,
                    EmpLastName = e.EmpLastName,
                    EmpGender = e.EmpGender,
                    EmpDob = e.EmpDob,  // Return as DateTime? instead of converting to string
                    EmpPhoneNumber = e.EmpPhoneNumber,
                    EmpEmail = e.EmpEmail,
                    EmpPassword = e.EmpPassword, // Ensure that password is not returned for security reasons
                    EmpAddress = e.EmpAddress,
                    EmpWard = e.EmpWard,
                    EmpStartDate = e.EmpStartDate,  // Return as DateTime? instead of converting to string
                    EmpStatus = e.EmpStatus,
                    RoleId = e.EmpRole.RoleId  // RoleId should match the RoleId property in EmployeeCreateDTO
                })
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPut("/api/updateEmployee/{id}")]
        public async Task<IActionResult> PutEmployee(int id, EmployeeUpdateDTO employeeDto)
        {
            var existingEmployee = await _context.Employees
                .Include(e => e.EmpRole)
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

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

            
            if (!string.IsNullOrEmpty(employeeDto.RoleName))
            {
                var roleName = employeeDto.RoleName;
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);

                if (role != null)
                {
                    existingEmployee.EmpRoleId = role.RoleId; 
                    existingEmployee.EmpRole = role; 
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
