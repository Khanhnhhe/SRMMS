using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointListsController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public PointListsController(SRMMSContext context)
        {
            _context = context;
        }

        // GET: api/PointLists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointDTO>>> GetPointLists()
        {
            if (_context.PointLists == null)
            {
                return NotFound();
            }

            // Sử dụng DTO để chỉ trả về các thông tin cần thiết
            return await _context.PointLists
                .Include(p => p.Order) // Join với bảng Order
                .Select(p => new PointDTO
                {
                    PointId = p.PointId,
                    OrderId = (int)p.OrderId,
                    NumberPoint = (int)p.NumberPonit,
                    OrderDate = (DateTime)p.Order.OrderDate,
                    TotalMoney = (decimal)p.Order.TotalMoney
                })
                .ToListAsync();
        }

        // GET: api/PointLists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PointDTO>> GetPointList(int id)
        {
            if (_context.PointLists == null)
            {
                return NotFound();
            }

            var point = await _context.PointLists
                .Include(p => p.Order) // Join với Order
                .Where(p => p.PointId == id)
                .Select(p => new PointDTO
                {
                    PointId = p.PointId,
                    OrderId = (int)p.OrderId,
                    NumberPoint = (int)p.NumberPonit,
                    OrderDate = (DateTime)p.Order.OrderDate,
                    TotalMoney = (decimal)p.Order.TotalMoney
                })
                .FirstOrDefaultAsync();

            if (point == null)
            {
                return NotFound();
            }

            return point;
        }

        // POST: api/PointLists/CalculatePoints
        [HttpPost("CalculatePoints/{orderId}")]
        public async Task<ActionResult<PointDTO>> CalculateRewardPoints(int orderId)
        {
            if (_context.Orders == null || _context.PointLists == null)
            {
                return Problem("Entity set 'SRMMSContext.Orders' is null.");
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return BadRequest("Order not found or not paid.");
            }

            // Tính điểm thưởng (VD: 1 điểm cho mỗi 100,000 VNĐ)
            int points = (int)(order.TotalMoney / 100000);

            // Thêm điểm thưởng vào bảng PointList
            var point = new PointList
            {
                AccId = order.AccId, 
                OrderId = orderId,
                NumberPonit = points
            };

            _context.PointLists.Add(point);
            await _context.SaveChangesAsync();

            // Trả về DTO
            return CreatedAtAction("GetPointList", new { id = point.PointId }, new PointDTO
            {
                PointId = point.PointId,
                OrderId = (int)point.OrderId,
                NumberPoint = (int)point.NumberPonit,
                OrderDate = (DateTime)order.OrderDate,
                TotalMoney = (decimal)order.TotalMoney
            });
        }

        // PUT: api/PointLists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPointList(int id, PointList pointList)
        {
            if (id != pointList.PointId)
            {
                return BadRequest();
            }

            _context.Entry(pointList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PointListExists(id))
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

        // DELETE: api/PointLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePointList(int id)
        {
            if (_context.PointLists == null)
            {
                return NotFound();
            }
            var pointList = await _context.PointLists.FindAsync(id);
            if (pointList == null)
            {
                return NotFound();
            }

            _context.PointLists.Remove(pointList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PointListExists(int id)
        {
            return (_context.PointLists?.Any(e => e.PointId == id)).GetValueOrDefault();
        }
    }
}
