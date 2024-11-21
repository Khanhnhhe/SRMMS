using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;

namespace SRMMS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
	{
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
		{
            _orderService = orderService;
        }

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] OrderDTO orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Order data is required.");
            }

            var orderId = await _orderService.CreateOrder(orderDto);
            return Ok(new { OrderId = orderId });
        }
        [HttpGet("list")]
        public IActionResult GetOrders(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? tableName = null,
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null)
        {
            try
            {
                // Gọi service để lấy dữ liệu đơn hàng đã phân trang và tính toán tổng số đơn hàng
                var (orders, totalOrders) = _orderService.GetOrders(pageNumber, pageSize, tableName, fromDate, toDate);

                // Trả về kết quả với danh sách đơn hàng và tổng số đơn hàng
                return Ok(new { Orders = orders, TotalOrders = totalOrders });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có exception xảy ra
                return BadRequest(new { Message = ex.Message });
            }
        }



        [HttpGet("listOrderByTable/{tableId}")]
        public IActionResult GetOrdersByTable(int tableId, int pageNumber = 1, int pageSize = 10)
        {
            var orders = _orderService.GetOrdersByTable(tableId, pageNumber, pageSize);
            if (orders == null || orders.Count == 0)
            {
                return NotFound(new { Message = $"No orders found for table {tableId}" });
            }
            return Ok(orders);
        }

        [HttpGet("searchByTableName")]
        public IActionResult SearchOrdersByTableName([FromQuery] string? tableName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            if (string.IsNullOrEmpty(tableName))
            {
                var orders = _orderService.GetOrders(pageNumber, pageSize);
                return Ok(orders);
            }
            else
            {

                var orders = _orderService.SearchOrdersByTableName(tableName, pageNumber, pageSize);
                return Ok(orders);
            }
        }

        [HttpGet("count")]
        public IActionResult CountOrders()
        {
            try
            {
                int totalOrders = _orderService.CountOrders();
                return Ok(new { TotalOrders = totalOrders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while counting orders.", Error = ex.Message });
            }
        }

    }
}

