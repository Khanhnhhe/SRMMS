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
    public class DiscountCodesController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public DiscountCodesController(SRMMSContext context)
        {
            _context = context;
        }

        // GET: api/DiscountCodes
        [HttpGet ("list")]
        public async Task<ActionResult<IEnumerable<DiscountCodeDto>>> GetDiscountCodes(int pageNumber = 1, int pageSize = 10)
        {
        

            pageSize = pageSize > 0 ? pageSize : 10;
            pageNumber = pageNumber > 0 ? pageNumber : 1;
            var discountCodes = await _context.DiscountCodes
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(d => new DiscountCodeDto
        {
            CodeId = d.CodeId,
            CodeDetail = d.CodeDetail,
            DiscountValue = d.DiscountValue,
            StartDate = d.StartDate.Value.ToString("dd/MM/yyyy"),
            EndDate = d.EndDate.Value.ToString("dd/MM/yyyy"),
            Status = d.Status
        })
                .ToListAsync();

            if (!discountCodes.Any())
            {
                return NotFound("No discount codes available.");
            }

            return Ok(discountCodes);
        }


        // GET: api/DiscountCodes/5
        [HttpGet("getByID/{id}")]
        public async Task<ActionResult<DiscountCodeDto>> GetDiscountCode(int id)
        {
            if (_context.DiscountCodes == null)
            {
                return NotFound("No discount codes available.");
            }

            var discountCode = await _context.DiscountCodes.FindAsync(id);

            if (discountCode == null)
            {
                return NotFound($"Discount code with ID {id} not found.");
            }

        
            var discountCodeDto = new DiscountCodeDto
            {
                CodeId = discountCode.CodeId,
                CodeDetail = discountCode.CodeDetail,
                DiscountValue = discountCode.DiscountValue,
                StartDate = discountCode.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = discountCode.EndDate?.ToString("dd/MM/yyyy"),
                Status = discountCode.Status
            };

            return Ok(discountCodeDto);
        }

        // PUT: api/DiscountCodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscountCode(int id, DiscountCode discountCode)
        {
            if (id != discountCode.CodeId)
            {
                return BadRequest();
            }

            _context.Entry(discountCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountCodeExists(id))
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

        // POST: api/DiscountCodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/DiscountCodes
        [HttpPost("createDiscountCode")]
        public async Task<ActionResult<DiscountCodeDto>> PostDiscountCode(DiscountCodeCreateDto discountCodeDto)
        {
            if (_context.DiscountCodes == null)
            {
                return Problem("Entity set 'SRMMSContext.DiscountCodes' is null.");
            }

            // Validate StartDate and EndDate
            if (!IsDateRangeValid(discountCodeDto.StartDate, discountCodeDto.EndDate))
            {
                return BadRequest("StartDate must be earlier than or equal to EndDate.");
            }

            // Map DTO to DiscountCode entity
            var discountCode = new DiscountCode
            {
                CodeDetail = discountCodeDto.CodeDetail,
                DiscountValue = discountCodeDto.DiscountValue,
                StartDate = discountCodeDto.StartDate,
                EndDate = discountCodeDto.EndDate,
                Status = discountCodeDto.Status
            };

            _context.DiscountCodes.Add(discountCode);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DiscountCodeExists(discountCode.CodeId))
                {
                    return Conflict("Discount code with the same ID already exists.");
                }
                else
                {
                    throw;
                }
            }

            // Map to DiscountCodeDto for response
            var responseDto = new DiscountCodeDto
            {
                CodeId = discountCode.CodeId,
                CodeDetail = discountCode.CodeDetail,
                DiscountValue = discountCode.DiscountValue,
                StartDate = discountCode.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = discountCode.EndDate?.ToString("dd/MM/yyyy"),
                Status = discountCode.Status
            };

            return CreatedAtAction("GetDiscountCode", new { id = discountCode.CodeId }, responseDto);
        }


        // Validation method for date range
        private bool IsDateRangeValid(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return false;
            }
            return startDate < endDate;
        }

        // DELETE: api/DiscountCodes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DiscountCodeDto>> DeleteDiscountCode(int id)
        {
            if (_context.DiscountCodes == null)
            {
                return Problem("Entity set 'SRMMSContext.DiscountCodes' is null.");
            }

            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound($"Discount code with ID {id} not found.");
            }

            var deletedDiscountCodeDto = new DiscountCodeDto
            {
                CodeId = discountCode.CodeId,
                CodeDetail = discountCode.CodeDetail,
                DiscountValue = discountCode.DiscountValue,
                StartDate = discountCode.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = discountCode.EndDate?.ToString("dd/MM/yyyy"),
                Status = discountCode.Status
            };

            _context.DiscountCodes.Remove(discountCode);
            await _context.SaveChangesAsync();

            return Ok(deletedDiscountCodeDto);
        }

        private bool DiscountCodeExists(int id)
        {
            return (_context.DiscountCodes?.Any(e => e.CodeId == id)).GetValueOrDefault();
        }
    }
}
