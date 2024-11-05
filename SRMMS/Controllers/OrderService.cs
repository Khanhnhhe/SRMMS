using System;
using Microsoft.AspNetCore.SignalR;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
	public class OrderService
	{
        private readonly SRMMSContext _context;
        private readonly IHubContext<OrderHub> _orderHubContext;

        public OrderService(SRMMSContext context, IHubContext<OrderHub> orderHubContext)
        {
            _context = context;
            _orderHubContext = orderHubContext;
        }
        public async Task<int> CreateOrder(OrderDTO orderDto)
        {
            var order = new Order
            {
                TableId = orderDto.TableId,
                OrderDate = DateTime.Now,
                TotalMoney = orderDto.TotalMoney,
                Status = orderDto.Status,
                CodeId = orderDto.CodeId,
                OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    ProId = od.ProId,
                    Quantiity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await _orderHubContext.Clients.All.SendAsync("ReceiveOrder", order);

            return order.OrderId;
        }
	}
}

