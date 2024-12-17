using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
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
        [HttpGet("list")]
        public async Task<ActionResult<object>> GetDiscountCodes(
     int pageNumber = 1,
     int pageSize = 10,
     decimal? minDiscountCodeValue = null,
     decimal? maxDiscountCodeValue = null,
     string? codeDetail = null,
     string? startDate = null,
     string? endDate = null)
        {
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Số trang và số lượng trên mỗi trang phải lớn hơn 0."
                    });
                }
                if (minDiscountCodeValue.HasValue && minDiscountCodeValue < 0)
                {
                    return BadRequest(new
                    {
                        message = "Giá trị mã giảm giá tối thiểu phải lớn hơn hoặc bằng 0."
                    });
                }

                if (maxDiscountCodeValue.HasValue && maxDiscountCodeValue < 0)
                {
                    return BadRequest(new
                    {
                        message = "Giá trị mã giảm giá tối đa phải lớn hơn hoặc bằng 0."
                    });
                }

                if (minDiscountCodeValue.HasValue && maxDiscountCodeValue.HasValue && minDiscountCodeValue > maxDiscountCodeValue)
                {
                    return BadRequest(new
                    {
                        message = "Giá trị tối thiểu không thể lớn hơn giá trị tối đa."
                    });
                }

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    if (DateTime.Parse(startDate) > DateTime.Parse(endDate))
                    {
                        return BadRequest(new
                        {
                            message = "Ngày bắt đầu không thể lớn hơn ngày kết thúc."
                        });
                    }
                }

                var query = _context.DiscountCodes.AsQueryable();

                if (!string.IsNullOrEmpty(codeDetail))
                {
                    codeDetail = codeDetail.Trim().ToLower();

                    query = query.Where(d => d.CodeDetail.Contains(codeDetail));
                }

                if (minDiscountCodeValue.HasValue)
                {
                    query = query.Where(d => (decimal)d.DiscountValue >= minDiscountCodeValue.Value);
                }
                if (maxDiscountCodeValue.HasValue)
                {
                    query = query.Where(d => (decimal)d.DiscountValue <= maxDiscountCodeValue.Value);
                }

                if (!string.IsNullOrEmpty(startDate))
                {
                    query = query.Where(d => d.StartDate >= DateTime.Parse(startDate));
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    query = query.Where(d => d.EndDate <= DateTime.Parse(endDate));
                }

                int totalDiscountCode = await query.CountAsync();

                var discountCodes = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(d => new DiscountCodeDto
                    {
                        CodeId = d.CodeId,
                        CodeDetail = d.CodeDetail,
                        DiscountValue = d.DiscountValue,
                        StartDate = d.StartDate.HasValue ? d.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                        EndDate = d.EndDate.HasValue ? d.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                        Status = d.Status,
                        DiscountType = d.DiscountType ?? 0
                    })
                    .ToListAsync();

                if (!discountCodes.Any())
                {
                    return BadRequest(new
                    {
                        message = "Không có mã giảm giá nào phù hợp với tiêu chí tìm kiếm."
                    });
                }

                return Ok(new
                {
                    totalDiscountCode,
                    discountCodes,
                    pageNumber,
                    pageSize,
                    minDiscountCodeValue,
                    maxDiscountCodeValue,
                    codeDetail,
                    startDate,
                    endDate
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi cơ sở dữ liệu khi lấy danh sách mã giảm giá.",
                    chiTiet = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi không xác định.",
                    chiTiet = ex.Message
                });
            }
        }


        // GET: api/DiscountCodes/5
        [HttpGet("getByID/{id}")]
        public async Task<ActionResult<DiscountCodeDto>> GetDiscountCode(int id)
        {
            if (_context.DiscountCodes == null)
            {
                return BadRequest(new
                {
                    message = "Không tìm thấy danh sách mã giảm giá."
                });
            }

            var discountCode = await _context.DiscountCodes.FindAsync(id);

            if (discountCode == null)
            {
                return BadRequest(new
                {
                    message = $"Không tìm thấy mã giảm giá với ID {id}."
                });
            }

            var discountCodeDto = new DiscountCodeDto
            {
                CodeId = discountCode.CodeId,
                CodeDetail = discountCode.CodeDetail,
                DiscountValue = discountCode.DiscountValue,
                StartDate = discountCode.StartDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
                EndDate = discountCode.EndDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
                Status = discountCode.Status,
                DiscountType = discountCode.DiscountType ?? 0
            };

            return Ok(discountCodeDto);
        }


        // PUT: api/DiscountCodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update/{id}")]
        public async Task<ActionResult<DiscountCodeDto>> UpdateDiscountCode(int id, DiscountCodeCreateDto discountCodeDto)
        {
            if (_context.DiscountCodes == null)
            {
                return Problem("Entity set 'SRMMSContext.DiscountCodes' is null.");
            }

            var existingDiscountCode = await _context.DiscountCodes.FindAsync(id);
            if (existingDiscountCode == null)
            {
                return NotFound(new { message = "Mã giảm giá không tồn tại." });
            }

            // Check if the new CodeDetail already exists (if it's different from the existing one)
            if (existingDiscountCode.CodeDetail != discountCodeDto.CodeDetail)
            {
                var isCodeDetailExists = await _context.DiscountCodes
                    .AnyAsync(dc => dc.CodeDetail == discountCodeDto.CodeDetail);

                if (isCodeDetailExists)
                {
                    return Conflict(new { message = "Mã giảm giá đã tồn tại. Vui lòng sử dụng mã khác." });
                }
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(discountCodeDto.CodeDetail))
            {
                return BadRequest(new { message = "Chi tiết mã giảm giá không được để trống." });
            }

            if (discountCodeDto.DiscountValue <= 0)
            {
                return BadRequest(new { message = "Giá trị giảm giá phải lớn hơn 0." });
            }

            if (discountCodeDto.StartDate == null || discountCodeDto.EndDate == null)
            {
                return BadRequest(new { message = "Ngày bắt đầu hoặc ngày kết thúc không được để trống." });
            }

            if (!IsDateRangeValid(discountCodeDto.StartDate, discountCodeDto.EndDate))
            {
                return BadRequest(new { message = "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc." });
            }

            if (discountCodeDto.DiscountType == DiscountType.Percentage && discountCodeDto.DiscountValue > 100)
            {
                return BadRequest(new { message = "Giảm giá phần trăm không được vượt quá 100%." });
            }

            // Update the existing discount code
            existingDiscountCode.CodeDetail = discountCodeDto.CodeDetail;
            existingDiscountCode.DiscountValue = discountCodeDto.DiscountValue;
            existingDiscountCode.StartDate = discountCodeDto.StartDate;
            existingDiscountCode.EndDate = discountCodeDto.EndDate;
            existingDiscountCode.Status = discountCodeDto.Status;
            existingDiscountCode.DiscountType = (int?)discountCodeDto.DiscountType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!DiscountCodeExists(existingDiscountCode.CodeId))
                {
                    return NotFound(new { message = "Mã giảm giá không tồn tại." });
                }
                else
                {
                    throw;
                }
            }

            var responseDto = new DiscountCodeDto
            {
                CodeId = existingDiscountCode.CodeId,
                CodeDetail = existingDiscountCode.CodeDetail,
                DiscountValue = existingDiscountCode.DiscountValue,
                StartDate = existingDiscountCode?.StartDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
                EndDate = existingDiscountCode?.EndDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
                Status = existingDiscountCode.Status,
                DiscountType = existingDiscountCode.DiscountType ?? 0
            };

            return Ok(responseDto);
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
            var isCodeDetailExists = await _context.DiscountCodes
       .AnyAsync(dc => dc.CodeDetail == discountCodeDto.CodeDetail);

            if (isCodeDetailExists)
            {
                return Conflict(new { message = "Mã giảm giá đã tồn tại. Vui lòng sử dụng mã khác." });
            }
          
            if (string.IsNullOrWhiteSpace(discountCodeDto.CodeDetail))
            {
                return BadRequest(new { message = "Chi tiết mã giảm giá không được để trống." });
            }

            if (discountCodeDto.DiscountValue <= 0)
            {
                return BadRequest(new { message = "Giá trị giảm giá phải lớn hơn 0." });
            }
            if (discountCodeDto.StartDate == null || discountCodeDto.EndDate == null)
            {
                return BadRequest(new { message = "Ngày bắt đầu hoặc ngày kết thúc không được để trống." });
            }

            if (!IsDateRangeValid(discountCodeDto.StartDate, discountCodeDto.EndDate))
            {
                return BadRequest(new { message = "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc." });
            }

            if (discountCodeDto.DiscountType == DiscountType.Percentage && discountCodeDto.DiscountValue > 100)
            {
                return BadRequest(new { message = "Giảm giá phần trăm không được vượt quá 100%." });
            }

            var discountCode = new DiscountCode
            {
                CodeDetail = discountCodeDto.CodeDetail,
                DiscountValue = discountCodeDto.DiscountValue,
                StartDate = discountCodeDto.StartDate, 
                EndDate = discountCodeDto.EndDate,     
                Status = discountCodeDto.Status,
                DiscountType = (int?)discountCodeDto.DiscountType
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
                    return Conflict("Mã giảm giá này đã tồn tại.");
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
                StartDate = discountCode?.StartDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
                EndDate = discountCode?.EndDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
                Status = discountCode.Status,
                DiscountType = discountCode.DiscountType ?? 0
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
                return BadRequest(new
                {
                    message = $"Không tìm thấy mã giảm giá với ID {id}"
                });
            }

            discountCode.Status = false;
            await _context.SaveChangesAsync();

            var updatedDiscountCodeDto = new DiscountCodeDto
            {
                CodeId = discountCode.CodeId,
                CodeDetail = discountCode.CodeDetail,
                DiscountValue = discountCode.DiscountValue,
                StartDate = discountCode.StartDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
                EndDate = discountCode.EndDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
                Status = discountCode.Status
            };

            return Ok(updatedDiscountCodeDto);
        }

        private bool DiscountCodeExists(int id)
        {
            return (_context.DiscountCodes?.Any(e => e.CodeId == id)).GetValueOrDefault();
        }
    }


}

