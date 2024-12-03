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
                return BadRequest("Dữ liệu đơn hàng là bắt buộc.");
            }

            var orderId = await _orderService.CreateOrder(orderDto);
            return Ok(new { OrderId = orderId });
        }

        [HttpPut("staffUpdate/{tableId}")]
        public async Task<IActionResult> UpdateOrder(int tableId, [FromBody] ComfirmOrderDTO orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Dữ liệu đơn hàng không hợp lệ.");
            }

            try
            {
               
                int updatedOrderId = await _orderService.ConfirmOrder(tableId, orderDto);

               
                return Ok(new { OrderId = updatedOrderId });
            }
            catch (Exception ex)
            {
               
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("kitchen-status/{orderId}")]
        public async Task<IActionResult> ChangeOrderStatusToComplete(int orderId)
        {
            try
            {
               
                int updatedOrderId = await _orderService.ChangeOrderStatusToComplete(orderId);

                
                return Ok(new { message = "Trạng thái đơn hàng đã được thay đổi", orderId = updatedOrderId });
            }
            catch (Exception ex)
            {
                
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("listStatus2")]
        public IActionResult GetOrdersStatus2(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string? tableName = null,
     [FromQuery] String? fromDate = null,
     [FromQuery] String? toDate = null)
        {
            try
            {

                var result = _orderService.GetOrdersStatus2(pageNumber, pageSize, tableName, fromDate, toDate);


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
        [HttpGet("listStatus4")]
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
                    return Ok(new { Message = $"Không tìm thấy đơn hàng có ID {orderId}." });
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpGet("listOrderByTable/{tableId}")]
        public IActionResult GetOrdersByTable(int tableId)
        {
            try
            {
                var orders = _orderService.GetOrdersByTable(tableId);

                if (orders == null || orders.Count == 0)
                {
                    return Ok(new { Message = $"Không tìm thấy đơn hàng nào cho bàn với ID {tableId}" });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi đếm đơn hàng.", Error = ex.Message });
            }
        }
        [HttpPost("CompleteOrder/{orderId}")]
        public async Task<IActionResult> CompleteOrder(int orderId, [FromBody] CompleteOrderDTO completeOrderDto)
        {
            try
            {
                var result = await _orderService.CompleteOrder(orderId, completeOrderDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
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

