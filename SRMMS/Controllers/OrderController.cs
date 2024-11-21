using System;
using Microsoft.AspNetCore.Mvc;
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
        //[HttpGet("list")]
        //public IActionResult GetOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? tableName = null)
        //{
        //    var orders = _orderService.GetOrders(pageNumber, pageSize, tableName);
        //    return Ok(orders);
        //}

        //[HttpGet("listOrderByTable/{tableId}")]
        //public IActionResult GetOrdersByTable(int tableId, int pageNumber = 1, int pageSize = 10)
        //{
        //    var orders = _orderService.GetOrdersByTable(tableId, pageNumber, pageSize);
        //    if (orders == null || orders.Count == 0)
        //    {
        //        return NotFound(new { Message = $"No orders found for table {tableId}" });
        //    }
        //    return Ok(orders);
        //}

        //[HttpGet("searchByTableName")]
        //public IActionResult SearchOrdersByTableName([FromQuery] string? tableName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{

        //    if (string.IsNullOrEmpty(tableName))
        //    {
        //        var orders = _orderService.GetOrders(pageNumber, pageSize);
        //        return Ok(orders);
        //    }
        //    else
        //    {

        //        var orders = _orderService.SearchOrdersByTableName(tableName, pageNumber, pageSize);
        //        return Ok(orders);
        //    }
        //}
    }
}

