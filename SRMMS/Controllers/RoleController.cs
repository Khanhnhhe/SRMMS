using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public RoleController(SRMMSContext context)
        {
            _context = context;
        }


        [HttpGet("/api/list-roles")]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> getListRoles()
        {
            try
            {
                var roles = await _context.Roles
               .Select(e => new RoleDTO
               {
                   RoleId = e.RoleId,
                   RoleName = e.RoleName,
                   Description = e.Description,
               })
               .ToListAsync();

                if (roles == null)
                {
                    return NotFound();
                }

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
