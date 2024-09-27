using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
