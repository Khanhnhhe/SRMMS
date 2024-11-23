using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
           
           
            
            var table = await _context.Tables.FindAsync(orderDto.TableId);
            if (table == null)
            {
                throw new Exception("TableId không tồn tại.");
            }
            
            var order = new Order
            {
                TableId = orderDto.TableId,
                TotalMoney = orderDto.TotalMoney,
                Status = orderDto.Status,
                OrderDetails = new List<OrderDetail>()
            };

            bool hasCombo = false;
            bool hasProduct = false;

            
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

                    if (combo.ComboStatus == false)
                    {
                        throw new Exception($"Combo với ID {comboDetail.ComboId} đã bị vô hiệu hóa, không thể đặt hàng.");
                    }

                    var orderDetail = new OrderDetail
                    {
                        ComboId = comboDetail.ComboId,
                        Quantiity = comboDetail.Quantity,
                        Price = comboDetail.Price
                    };

                    order.OrderDetails.Add(orderDetail);
                }
                hasCombo = true;
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

                    if (product.ProStatus == false)
                    {
                        throw new Exception($"Sản phẩm với ID {productDetail.ProId} đã bị vô hiệu hóa, không thể đặt hàng.");
                    }

                    var orderDetail = new OrderDetail
                    {
                        ProId = productDetail.ProId,
                        Quantiity = productDetail.Quantity,
                        Price = productDetail.Price
                    };

                    order.OrderDetails.Add(orderDetail);
                }
                hasProduct = true;
            }

            
            if (!hasCombo && !hasProduct)
            {
                throw new Exception("Đơn hàng phải có ít nhất một sản phẩm hoặc combo.");
            }

            
            order.TotalMoney = orderDto.TotalMoney;

            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _orderHubContext.Clients.All.SendAsync("ReceiveOrder", order);

            return order.OrderId;
        }



        public (List<GetOrderByTableNameDTO> Orders, int TotalOrders) GetOrders(
        int pageNumber = 1,
        int pageSize = 10,
        string? tableName = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
        {
            var query = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Pro)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Combo)
                .AsQueryable();

          
            if (!string.IsNullOrEmpty(tableName))
            {
                string normalizedTableName = tableName.Replace(" ", "");
                var tableExists = _context.Tables
                    .Any(t => t.TableName.Replace(" ", "").Contains(normalizedTableName));

                if (!tableExists)
                {
                    throw new Exception($"Table with name '{tableName}' does not exist.");
                }

                query = query.Where(o => o.Table.TableName.Replace(" ", "").Contains(normalizedTableName));
            }

          
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= toDate.Value);
            }

            
            var totalOrders = query.Count();

            
            var orders = query
                .Select(o => new GetOrderByTableNameDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalMoney = o.TotalMoney,
                    Status = o.Status,
                    TableId = o.Table.TableId,
                    TableName = o.Table.TableName,
                    Products = o.OrderDetails
                        .Where(od => od.Pro != null)
                        .Select(od => new GetProductDTO
                        {
                            ProductId = od.Pro.ProId,
                            Quantity = (int)od.Quantiity,
                            ProName = od.Pro.ProName,
                            Price = od.Price
                        }).ToList(),
                    Combos = o.OrderDetails
                        .Where(od => od.Combo != null)
                        .Select(od => new GetComboDTO
                        {
                            ComboId = od.Combo.ComboId,
                            Quantity = (int)od.Quantiity,
                            ComboName = od.Combo.ComboName,
                            Price = od.Price
                        }).ToList()
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (orders, totalOrders);  
        }



        public GetOrderByOrderIdDTO GetOrderByOrderId(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Pro)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                .Include(o => o.PointLists)
                    .ThenInclude(pl => pl.Acc)
                .Include(o => o.Code)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} does not exist.");
            }

            return new GetOrderByOrderIdDTO
            {
                OrderId = order.OrderId,
                OrderDate = (DateTime)order.OrderDate,
                TotalMoney = (double)order.TotalMoney,
                Status = (bool)order.Status,
                TableId = order.Table.TableId,
                Products = order.OrderDetails
                    .Where(od => od.Pro != null)
                    .Select(od => new GetProductDTO
                    {
                        ProductId = od.Pro.ProId,
                        Quantity = (int)od.Quantiity,
                        ProName = od.Pro.ProName,
                        Price = od.Price
                    }).ToList(),
                Combos = order.OrderDetails
                    .Where(od => od.Combo != null)
                    .Select(od => new GetComboDTO
                    {
                        ComboId = od.Combo.ComboId,
                        Quantity = (int)od.Quantiity,
                        ComboName = od.Combo.ComboName,
                        Price = od.Price
                    }).ToList(),
                Customers = order.PointLists
                    .Where(pl => pl.Acc != null)
                    .Select(pl => new GetAccountDTO
                    {
                        AccId = pl.Acc.AccId,
                        FullName = pl.Acc.FullName,
                        Email = pl.Acc.Email,
                        Phone = pl.Acc.Phone
                    }).ToList(),
                DiscountId = order.Code?.CodeId,
                DiscountValue = order.Code?.DiscountValue,
               
                PointIds = order.PointLists?.Select(pl => pl.PointId).ToList() ?? new List<int>(),
                PointNumbers = order.PointLists?.Select(pl => (double?)pl.NumberPonit).ToList() ?? new List<double?>() 
            };
        
        }




        public List<GetOrderByTableNameDTO> GetOrdersByTable(int tableId, int pageNumber = 1, int pageSize = 10)
        {
            var tableExists = _context.Tables.Any(t => t.TableId == tableId);

            if (!tableExists)
            {
                
                throw new Exception($"Table with ID '{tableId}' does not exist.");
            }

            var query = _context.Orders
                .Where(o => o.TableId == tableId)
                .Include(o => o.OrderDetails)  
                    .ThenInclude(od => od.Pro)  
                .Include(o => o.OrderDetails)  
                    .ThenInclude(od => od.Combo)  
                .AsQueryable();

            
            var orders = query
                .Select(o => new GetOrderByTableNameDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalMoney = o.TotalMoney,
                    Status = o.Status,
                    Products = o.OrderDetails
                        .Where(od => od.Pro != null)  
                        .Select(od => new GetProductDTO
                        {
                            ProductId = od.Pro.ProId,
                            Quantity = (int)od.Quantiity,
                            ProName = od.Pro.ProName,
                            Price = od.Price
                        }).ToList(),
                    Combos = o.OrderDetails
                        .Where(od => od.Combo != null)  
                        .Select(od => new GetComboDTO
                        {
                            ComboId = od.Combo.ComboId,
                            Quantity = (int)od.Quantiity,
                            ComboName = od.Combo.ComboName,
                            Price = od.Price
                        }).ToList()
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return orders;
        }



        public List<GetOrderByTableNameDTO> SearchOrdersByTableName(string tableName, int pageNumber = 1, int pageSize = 10)
        {
            
            string normalizedTableName = tableName.Replace(" ", "");

            
            var tableExists = _context.Tables.Any(t => t.TableName.Replace(" ", "").Contains(normalizedTableName));

            if (!tableExists)
            {
                
                throw new Exception($"Table with name '{tableName}' does not exist.");
            }

           
            var query = _context.Orders
                .Include(o => o.Table)  
                .Include(o => o.OrderDetails)  
                    .ThenInclude(od => od.Pro)  
                .Include(o => o.OrderDetails)  
                    .ThenInclude(od => od.Combo)  
                .AsQueryable();

           
            query = query.Where(o => o.Table.TableName.Replace(" ", "").Contains(normalizedTableName));


            var orders = query
                .Select(o => new GetOrderByTableNameDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalMoney = o.TotalMoney,
                    Status = o.Status,
                    Products = o.OrderDetails
                        .Where(od => od.Pro != null)  
                        .Select(od => new GetProductDTO
                        {
                            ProductId = od.Pro.ProId,
                            Quantity = (int)od.Quantiity,
                            ProName = od.Pro.ProName,
                            Price = od.Price
                        }).ToList(),
                    Combos = o.OrderDetails
                        .Where(od => od.Combo != null)  
                        .Select(od => new GetComboDTO
                        {
                            ComboId = od.Combo.ComboId,
                            Quantity = (int)od.Quantiity,
                            ComboName = od.Combo.ComboName,
                            Price = od.Price
                        }).ToList()
                })
                .Skip((pageNumber - 1) * pageSize)  
                .Take(pageSize)                     
                .ToList();

            return orders;
        }
        public async Task CompleteOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .ThenInclude(t => t.StatusTable)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new Exception("Order not found.");
            }
           
            if (order.Status == true)
            {
                throw new Exception("This order has already been completed and cannot be modified.");
            }
            order.OrderDate = DateTime.Now;  
           
            if (order.Table != null)
            {
                
                order.Table.StatusId = 1;  
            }
            order.Status = true;       
            await _context.SaveChangesAsync();
        }

        public int CountOrders()
        {

            return _context.Orders.Count();
        }
    }
}


