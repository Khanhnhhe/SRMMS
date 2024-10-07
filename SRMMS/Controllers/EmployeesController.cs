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
                  EmpAdress = e.EmpAddress,
                   EmpPhoneNumber = e.EmpPhoneNumber,
                   EmpEmail = e.EmpEmail,
                   EmpStartDate = e.EmpStartDate.ToString("yyyy-MM-dd"),
                   EmpStatus = e.EmpStatus,
                   RoleName = e.EmpRole.RoleName 
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
            
            if (await _context.Employees.AnyAsync(e => e.EmpEmail == employeeDto.EmpEmail))
            {
                return BadRequest("An employee with this email already exists.");
            }

           
            employeeDto.RoleName = employeeDto.RoleName ?? "Staff"; 
            employeeDto.EmpStatus = employeeDto.EmpStatus ?? true; 
            employeeDto.EmpStartDate = employeeDto.EmpStartDate ?? DateTime.Now; 

            
            var newEmployee = new Employee
            {
                EmpFirstName = employeeDto.EmpFirstName,
                EmpLastName = employeeDto.EmpLastName,
                EmpDob = (DateTime)employeeDto.EmpDob,
                EmpPhoneNumber = (int)employeeDto.EmpPhoneNumber,
                EmpEmail = employeeDto.EmpEmail,
                EmpPassword = employeeDto.EmpPassword,
                EmpStartDate = employeeDto.EmpStartDate.Value, 
                EmpStatus = employeeDto.EmpStatus.Value 
            };

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == employeeDto.RoleName);
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
                RoleName = newEmployee.EmpRole?.RoleName
            };

            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmpId }, employeeResult);
        }



        [HttpGet("/api/getEmployeeByID/{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmpRole)
                .Select(e => new EmployeeDTO
                {
                    EmpId = e.EmpId,
                    EmpFirstName = e.EmpFirstName,
                    EmpLastName = e.EmpLastName,
                    EmpDob = e.EmpDob.Value.ToString("yyyy-MM-dd"),
                    EmpPhoneNumber = e.EmpPhoneNumber,
                    EmpEmail = e.EmpEmail,
                    EmpStartDate = e.EmpStartDate.ToString("yyyy-MM-dd"),
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
