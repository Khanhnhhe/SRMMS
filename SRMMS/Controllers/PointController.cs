using System;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {
        private readonly SRMMSContext _context;
        public PointController(SRMMSContext context)
        {
            _context = context;
        }

        [HttpPost("add-points")]
        public IActionResult AddPoints([FromQuery] int? accId, [FromQuery] string? phone, [FromQuery] int orderId)
        {

            if (!accId.HasValue && string.IsNullOrEmpty(phone))
            {
                return BadRequest(new { message = "Vui lòng cung cấp accId hoặc phone để thêm điểm." });
            }


            var order = _context.Orders
                .FirstOrDefault(o => o.OrderId == orderId && o.Status == true);

            if (order == null)
            {
                return Ok(new { message = "Order không tồn tại hoặc không hợp lệ." });
            }


            if (!order.TotalMoney.HasValue || order.TotalMoney <= 0)
            {
                return BadRequest(new { message = "TotalMoney không hợp lệ." });
            }


            var account = accId.HasValue
                ? _context.Accounts.FirstOrDefault(a => a.AccId == accId)
                : _context.Accounts.FirstOrDefault(a => a.Phone == phone);

            if (account == null)
            {
                return Ok(new { message = "Tài khoản không tồn tại." });
            }


            double points = (double)order.TotalMoney.Value / 100.0;
            points = Math.Floor(points);

            var pointEntry = _context.PointLists.FirstOrDefault(p => p.AccId == account.AccId);

            if (pointEntry != null)
            {

                pointEntry.NumberPonit = (pointEntry.NumberPonit ?? 0) + points;
            }
            else
            {

                _context.PointLists.Add(new PointList
                {
                    AccId = account.AccId,
                    OrderId = orderId,
                    NumberPonit = points
                });
            }


            _context.SaveChanges();


            return Ok(new
            {
                message = $"Đã thêm {points} điểm cho tài khoản {account.AccId} từ đơn hàng {orderId}.",
                fullName = account.FullName,
                phoneNumber = account.Phone,
                totalPoints = pointEntry?.NumberPonit ?? points
            });
        }


    }
}


