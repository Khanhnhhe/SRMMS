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

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<DiscountCodeDto>>> GetDiscountCodes(
            int pageNumber = 1,
            int pageSize = 10,
            string? codeDetail = null,
            double? discountValue = 0)
        {
            pageSize = pageSize > 0 ? pageSize : 10;
            pageNumber = pageNumber > 0 ? pageNumber : 1;

            await UpdateDiscountCodeStatusAsync();

            var query = _context.DiscountCodes.AsQueryable();

            if (!string.IsNullOrEmpty(codeDetail))
            {
                query = query.Where(d => d.CodeDetail.Contains(codeDetail));
            }

            if (discountValue > 0)
            {
                query = query.Where(d => d.DiscountValue == discountValue);
            }

            var discountCodes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var discountCodeDtos = discountCodes.Select(d => new DiscountCodeDto
            {
                CodeId = d.CodeId,
                CodeDetail = d.CodeDetail,
                DiscountValue = d.DiscountValue,
                StartDate = d.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = d.EndDate?.ToString("dd/MM/yyyy"),
                Status = d.Status
            }).ToList();

            if (!discountCodeDtos.Any())
            {
                return NotFound("No discount codes available.");
            }

            return Ok(discountCodeDtos);
        }



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

            // Kiểm tra trạng thái của mã giảm giá
            if (discountCode.EndDate.HasValue && discountCode.EndDate.Value.Date < DateTime.Today)
            {
                discountCode.Status = false;
                await _context.SaveChangesAsync();
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


        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutDiscountCode(int id, UpdateDiscountDTO discountCodeDto)
        {
            if (_context.DiscountCodes == null)
            {
                return Problem("Entity set 'SRMMSContext.DiscountCodes' is null.");
            }

            // Tìm thực thể DiscountCode theo ID
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound($"Discount code with ID {id} not found.");
            }

            // Ánh xạ dữ liệu từ DTO sang thực thể
            if (discountCodeDto.CodeDetail != null)
                discountCode.CodeDetail = discountCodeDto.CodeDetail;

            if (discountCodeDto.DiscountValue.HasValue)
                discountCode.DiscountValue = discountCodeDto.DiscountValue.Value;

            if (discountCodeDto.StartDate.HasValue)
                discountCode.StartDate = discountCodeDto.StartDate.Value.ToDateTime(TimeOnly.MinValue).Date;

            if (discountCodeDto.EndDate.HasValue)
                discountCode.EndDate = discountCodeDto.EndDate.Value.ToDateTime(TimeOnly.MinValue).Date;


            if (discountCodeDto.Status.HasValue)
                discountCode.Status = discountCodeDto.Status.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountCodeExists(id))
                {
                    return NotFound($"Discount code with ID {id} not found during update.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        [HttpPost("createDiscountCode")]
        public async Task<ActionResult<DiscountCodeDto>> PostDiscountCode(DiscountCodeCreateDto discountCodeDto)
        {
            if (_context.DiscountCodes == null)
            {
                return Problem("Entity set 'SRMMSContext.DiscountCodes' is null.");
            }

            if (!IsDateRangeValid(discountCodeDto.StartDate, discountCodeDto.EndDate))
            {
                return BadRequest("StartDate must be earlier than or equal to EndDate.");
            }

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


        private bool IsDateRangeValid(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return false;
            }
            return startDate < endDate;
        }

        [HttpPut("changeStatus/{id}")]
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

            discountCode.Status = false;
            await _context.SaveChangesAsync();

            var updatedDiscountCodeDto = new DiscountCodeDto
            {
                CodeId = discountCode.CodeId,
                CodeDetail = discountCode.CodeDetail,
                DiscountValue = discountCode.DiscountValue,
                StartDate = discountCode.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = discountCode.EndDate?.ToString("dd/MM/yyyy"),
                Status = discountCode.Status
            };

            return Ok(updatedDiscountCodeDto);
        }

        private bool DiscountCodeExists(int id)
        {
            return (_context.DiscountCodes?.Any(e => e.CodeId == id)).GetValueOrDefault();
        }
        private async Task UpdateDiscountCodeStatusAsync()
        {
            var expiredDiscounts = await _context.DiscountCodes
                .Where(d => d.EndDate.HasValue && d.EndDate.Value.Date < DateTime.Today && (bool)d.Status)
                .ToListAsync();

            foreach (var discount in expiredDiscounts)
            {
                discount.Status = false;
            }

            if (expiredDiscounts.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

    }
}
