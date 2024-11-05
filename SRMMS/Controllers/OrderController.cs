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
    }
}

