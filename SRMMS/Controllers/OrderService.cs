using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        public List<OrderDTO> GetOrders(int pageNumber = 1, int pageSize = 10, string? tableName = null)
        {
            var query = _context.Orders
                .Include(o => o.Table) 
                .AsQueryable();

            
            if (!string.IsNullOrEmpty(tableName))
            {
               
                string normalizedTableName = tableName.Replace(" ", "");

                query = query.Where(o => o.Table.TableName.Replace(" ", "").Contains(normalizedTableName)); 
            }

            var orders = query
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    TableId = o.TableId,
                    TableName = o.Table.TableName, 
                    OrderDate = o.OrderDate,
                    TotalMoney = o.TotalMoney,
                    Status = o.Status,
                    CodeId = o.CodeId,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                    {
                        ProId = od.ProId,
                        Quantity = od.Quantiity,
                        Price = od.Price
                    }).ToList()
                })
                .Skip((pageNumber - 1) * pageSize)  
                .Take(pageSize)                     
                .ToList();

            return orders;
        }


        public List<OrderDTO> GetOrdersByTable(int tableId, int pageNumber = 1, int pageSize = 10)
        {
            var ordersQuery = _context.Orders
                .Where(o => o.TableId == tableId)
                .OrderBy(o => o.OrderDate);

            var pagedOrders = ordersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    TableId = o.TableId,
                    OrderDate = o.OrderDate,
                    TotalMoney = o.TotalMoney,
                    Status = o.Status,
                    CodeId = o.CodeId,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                    {
                        ProId = od.ProId,
                        Quantity = od.Quantiity,
                        Price = od.Price
                    }).ToList()
                })
                .ToList();

            return pagedOrders;
        }

        public List<OrderDTO> SearchOrdersByTableName(string tableName, int pageNumber = 1, int pageSize = 10)
        {
            
            string normalizedTableName = tableName.Replace(" ", "");

            var query = _context.Orders
                .Include(o => o.Table) 
                .AsQueryable();

            
            query = query.Where(o => o.Table.TableName.Replace(" ", "").Contains(normalizedTableName));

            var orders = query
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    TableId = o.TableId,
                    TableName = o.Table.TableName, 
                    OrderDate = o.OrderDate,
                    TotalMoney = o.TotalMoney,
                    Status = o.Status,
                    CodeId = o.CodeId,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                    {
                        ProId = od.ProId,
                        Quantity = od.Quantiity,
                        Price = od.Price
                    }).ToList()
                })
                .Skip((pageNumber - 1) * pageSize)  
                .Take(pageSize)                     
                .ToList();

            return orders;
        }

        public int CountOrders()
        {
            
            return _context.Orders.Count();
        }
    }
}

