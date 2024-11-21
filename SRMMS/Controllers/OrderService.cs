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
           
            var code = await _context.DiscountCodes.FindAsync(orderDto.CodeId);
            if (code == null)
            {
                // Nếu không tìm thấy CodeId, trả về thông báo lỗi
                throw new Exception("CodeId không tồn tại.");
            }

           
            var table = await _context.Tables.FindAsync(orderDto.TableId);
            if (table == null)
            {
               
                throw new Exception("TableId không tồn tại.");
            }

           
            var order = new Order
            {
                TableId = orderDto.TableId,
                OrderDate = DateTime.Now,
                TotalMoney = orderDto.TotalMoney,  
                Status = orderDto.Status,
                CodeId = orderDto.CodeId,
                OrderDetails = new List<OrderDetail>()
            };

            
            if (orderDto.ComboDetails != null && orderDto.ComboDetails.Any())
            {
                foreach (var comboDetail in orderDto.ComboDetails)
                {
                    var combo = await _context.Combos
                        .FirstOrDefaultAsync(c => c.ComboId == comboDetail.ComboId);

                    if (combo == null)
                    {
                        throw new Exception($"Combo với ID {comboDetail.ComboId} không tồn tại.");
                    }

                    // Kiểm tra trạng thái của combo
                    if (combo.ComboStatus == false)
                    {
                        throw new Exception($"Combo với ID {comboDetail.ComboId} đã bị vô hiệu hóa, không thể đặt hàng.");
                    }
                    if (combo != null)
                    {
                       
                        var orderDetail = new OrderDetail
                        {
                            ComboId = comboDetail.ComboId,
                            Quantiity= comboDetail.Quantity,
                            Price = comboDetail.Price  
                        };

                        order.OrderDetails.Add(orderDetail);
                    }
                }
            }

           
            if (orderDto.ProductDetails != null && orderDto.ProductDetails.Any())
            {
                foreach (var productDetail in orderDto.ProductDetails)
                {
                    var product = await _context.Products.FindAsync(productDetail.ProId);
                    if (product == null)
                    {
                        throw new Exception($"Sản phẩm với ID {productDetail.ProId} không tồn tại.");
                    }

                    // Kiểm tra trạng thái của sản phẩm
                    if (product.ProStatus == false)
                    {
                        throw new Exception($"Sản phẩm với ID {productDetail.ProId} đã bị vô hiệu hóa, không thể đặt hàng.");
                    }
                    if (product != null)
                    {
                       
                        var orderDetail = new OrderDetail
                        {
                            ProId = productDetail.ProId,
                            Quantiity = productDetail.Quantity,
                            Price = productDetail.Price  
                        };

                        order.OrderDetails.Add(orderDetail);
                    }
                }
            }

            
            order.TotalMoney = orderDto.TotalMoney;

            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            
            await _orderHubContext.Clients.All.SendAsync("ReceiveOrder", order);

            
            return order.OrderId;
        }




        //    public List<OrderDTO> GetOrders(int pageNumber = 1, int pageSize = 10, string? tableName = null)
        //    {
        //        var query = _context.Orders
        //            .Include(o => o.Table) 
        //            .AsQueryable();


        //        if (!string.IsNullOrEmpty(tableName))
        //        {

        //            string normalizedTableName = tableName.Replace(" ", "");

        //            query = query.Where(o => o.Table.TableName.Replace(" ", "").Contains(normalizedTableName)); 
        //        }

        //        var orders = query
        //            .Select(o => new OrderDTO
        //            {
        //                OrderId = o.OrderId,
        //                TableId = o.TableId,
        //                TableName = o.Table.TableName, 
        //                OrderDate = o.OrderDate,
        //                TotalMoney = o.TotalMoney,
        //                Status = o.Status,
        //                CodeId = o.CodeId,
        //                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
        //                {
        //                    ProId = od.ProId,
        //                    Quantity = od.Quantiity,
        //                    Price = od.Price
        //                }).ToList()
        //            })
        //            .Skip((pageNumber - 1) * pageSize)  
        //            .Take(pageSize)                     
        //            .ToList();

        //        return orders;
        //    }


        //    public List<OrderDTO> GetOrdersByTable(int tableId, int pageNumber = 1, int pageSize = 10)
        //    {
        //        var ordersQuery = _context.Orders
        //            .Where(o => o.TableId == tableId)
        //            .OrderBy(o => o.OrderDate);

        //        var pagedOrders = ordersQuery
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .Select(o => new OrderDTO
        //            {
        //                OrderId = o.OrderId,
        //                TableId = o.TableId,
        //                OrderDate = o.OrderDate,
        //                TotalMoney = o.TotalMoney,
        //                Status = o.Status,
        //                CodeId = o.CodeId,
        //                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
        //                {
        //                    ProId = od.ProId,
        //                    Quantity = od.Quantiity,
        //                    Price = od.Price
        //                }).ToList()
        //            })
        //            .ToList();

        //        return pagedOrders;
        //    }

        //    public List<OrderDTO> SearchOrdersByTableName(string tableName, int pageNumber = 1, int pageSize = 10)
        //    {

        //        string normalizedTableName = tableName.Replace(" ", "");

        //        var query = _context.Orders
        //            .Include(o => o.Table) 
        //            .AsQueryable();


        //        query = query.Where(o => o.Table.TableName.Replace(" ", "").Contains(normalizedTableName));

        //        var orders = query
        //            .Select(o => new OrderDTO
        //            {
        //                OrderId = o.OrderId,
        //                TableId = o.TableId,
        //                TableName = o.Table.TableName, 
        //                OrderDate = o.OrderDate,
        //                TotalMoney = o.TotalMoney,
        //                Status = o.Status,
        //                CodeId = o.CodeId,
        //                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
        //                {
        //                    ProId = od.ProId,
        //                    Quantity = od.Quantiity,
        //                    Price = od.Price
        //                }).ToList()
        //            })
        //            .Skip((pageNumber - 1) * pageSize)  
        //            .Take(pageSize)                     
        //            .ToList();

        //        return orders;
        //    }

        //    public int CountOrders()
        //    {

        //        return _context.Orders.Count();
        //    }
    }
}


