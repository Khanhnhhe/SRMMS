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
     [FromQuery] String? fromDate = null,
     [FromQuery] String? toDate = null)
        {
            try
            {
               
                var result = _orderService.GetOrders(pageNumber, pageSize, tableName, fromDate, toDate);

              
                var orders = result.Orders;
                var totalOrders = result.TotalOrders;

               
                var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

                
                return Ok(new
                {
                    Orders = orders,
                    TotalOrders = totalOrders,
                    TotalPages = totalPages,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
               
                return BadRequest(new { Message = ex.Message });
            }
        }



        [HttpGet("order/{orderId}")]
        public IActionResult GetOrderByOrderId(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderByOrderId(orderId);
                if (order == null)
                {
                    return NotFound(new { Message = $"Order with ID {orderId} not found." });
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
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
        [HttpPost("complete-order/{orderId}")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            try
            {
               
                await _orderService.CompleteOrder(orderId);

               
                return Ok(new { Message = "Order completed successfully." });
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { Error = "An unexpected error occurred.", Detail = ex.Message });
            }
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue(
     [FromQuery] int? year = null,
     [FromQuery] int? month = null,
     [FromQuery] int? week = null)
        {
            try
            {
                // Gọi phương thức tính tổng doanh thu
                var result = await _orderService.CalculateTotalRevenue( week, month, year);

                return Ok(new
                {
                    TotalRevenue = result.TotalRevenue,
                    Orders = result.Orders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "An error occurred.",
                    Detail = ex.Message
                });
            }
        }


    }
}

