using System;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
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
            decimal calculatedTotalMoney = 0;


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
                    if (comboDetail.Quantity <= 0)
                    {
                        throw new Exception($"Số lượng combo với ID {comboDetail.ComboId} phải lớn hơn 0.");
                    }

                    var orderDetail = new OrderDetail
                    {
                        ComboId = comboDetail.ComboId,
                        Quantiity = comboDetail.Quantity,
                        Price = comboDetail.Price
                    };

                    calculatedTotalMoney += comboDetail.Quantity * comboDetail.Price;
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

                    if (productDetail.Quantity <= 0)
                    {
                        throw new Exception($"Số lượng sản phẩm với ID {productDetail.ProId} phải lớn hơn 0.");
                    }

                    if (order.TotalMoney <= 0)
                    {
                        throw new Exception("Tổng tiền phải lớn hơn 0.");
                    }
                    var orderDetail = new OrderDetail
                    {
                        ProId = productDetail.ProId,
                        Quantiity = productDetail.Quantity,
                        Price = productDetail.Price
                    };
                    calculatedTotalMoney += productDetail.Quantity * productDetail.Price;
                    order.OrderDetails.Add(orderDetail);
                }
                hasProduct = true;
            }


            if (!hasCombo && !hasProduct)
            {
                throw new Exception("Đơn hàng phải có ít nhất một sản phẩm hoặc combo.");
            }

            if (order.TotalMoney != calculatedTotalMoney)
            {
                throw new Exception($"Tổng tiền không khớp. Tổng tiền chính xác phải là {calculatedTotalMoney}.");
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
        String? fromDate = null,
        String? toDate = null)
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

            if (!string.IsNullOrEmpty(fromDate))
            {
                if (DateTime.TryParse(fromDate, out var parsedFromDate))
                {
                    query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= parsedFromDate);
                }
                else
                {
                    throw new Exception($"Invalid fromDate format: '{fromDate}'. Expected format is yyyy-MM-dd.");
                }
            }


            if (!string.IsNullOrEmpty(toDate))
            {
                if (DateTime.TryParse(toDate, out var parsedToDate))
                {
                    parsedToDate = parsedToDate.AddDays(1);
                    query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value < parsedToDate);
                }
                else
                {
                    throw new Exception($"Invalid toDate format: '{toDate}'. Expected format is yyyy-MM-dd.");
                }
            }
            var totalOrders = query.Count();


            var orders = query
                .Select(o => new GetOrderByTableNameDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
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
                TableName = order.Table.TableName,
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
                    OrderDate = o.OrderDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
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
                    OrderDate = o.OrderDate.Value.ToString("yyyy-MM-dd hh:mm:ss"),
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
        public async Task<OrderCompleteDTO> CompleteOrder(int orderId, int? discountId, decimal? totalMoney)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            if (order.Status == true)
            {
                throw new Exception("This order has already been completed and cannot be modified.");
            }

            double? discountValue = null;

           
            if (discountId.HasValue)
            {
                var discount = await _context.DiscountCodes.FirstOrDefaultAsync(d => d.CodeId == discountId.Value);

                if (discount == null)
                {
                    throw new Exception("Discount code not found.");
                }

                if (discount.Status != true || discount.StartDate > DateTime.Now || (discount.EndDate != null && discount.EndDate < DateTime.Now))
                {
                    throw new Exception("Discount code is invalid or expired.");
                }

                discountValue = discount.DiscountValue;

               
                if (totalMoney.HasValue)
                {

                    if (discountValue >= (double)totalMoney.Value)
                    {
                        totalMoney = 0; 
                    }
                }
            }

           
            if (totalMoney.HasValue)
            {
                order.TotalMoney = totalMoney.Value;
            }

           
            order.OrderDate = DateTime.Now;
            order.CodeId = discountId;

            if (order.Table != null)
            {
                order.Table.StatusId = 1; 
            }

            order.Status = true;

            await _context.SaveChangesAsync();

            
            return new OrderCompleteDTO
            {
                OrderId = order.OrderId,
                TotalMoney = order.TotalMoney, 
                TableId = order.TableId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                DiscountId = order.CodeId,
                DiscountValue = discountValue
            };
        }


        public async Task<(List<GetOrderByOrderIdDTO> Orders, decimal TotalRevenue)> CalculateTotalRevenue(int? weekNumber = null, int? month = null, int? year = null)
        {
            var query = _context.Orders
                                .Where(o => o.Status == true);


            if (weekNumber.HasValue && month.HasValue && year.HasValue)
            {
                var firstDayOfMonth = new DateTime(year.Value, month.Value, 1);
                DateTime startOfWeek;
                DateTime endOfWeek;


                switch (weekNumber.Value)
                {
                    case 1:
                        startOfWeek = firstDayOfMonth;
                        endOfWeek = firstDayOfMonth.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);  // Kết thúc tuần 1 vào cuối ngày 7
                        break;
                    case 2:
                        startOfWeek = firstDayOfMonth.AddDays(7);  // Bắt đầu tuần 2 từ 00:00 của ngày 8
                        endOfWeek = firstDayOfMonth.AddDays(13).AddHours(23).AddMinutes(59).AddSeconds(59);  // Kết thúc tuần 2 vào cuối ngày 14
                        break;
                    case 3:
                        startOfWeek = firstDayOfMonth.AddDays(14);  // Bắt đầu tuần 3 từ 00:00 của ngày 15
                        endOfWeek = firstDayOfMonth.AddDays(20).AddHours(23).AddMinutes(59).AddSeconds(59);  // Kết thúc tuần 3 vào cuối ngày 21
                        break;
                    case 4:
                        startOfWeek = firstDayOfMonth.AddDays(21);  // Bắt đầu tuần 4 từ 00:00 của ngày 22
                        endOfWeek = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);  // Kết thúc tuần 4 vào cuối ngày cuối tháng
                        break;
                    default:
                        throw new ArgumentException("Invalid week number. Please enter a value between 1 and 4.");
                }


                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                if (endOfWeek > lastDayOfMonth)
                {
                    endOfWeek = lastDayOfMonth;
                }

                query = query.Where(o => o.OrderDate >= startOfWeek && o.OrderDate <= endOfWeek);
            }

            else if (month.HasValue && year.HasValue)
            {
                var startOfMonth = new DateTime(year.Value, month.Value, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                query = query.Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth);
            }

            else if (year.HasValue)
            {
                var startOfYear = new DateTime(year.Value, 1, 1);
                var endOfYear = new DateTime(year.Value, 12, 31);

                query = query.Where(o => o.OrderDate >= startOfYear && o.OrderDate <= endOfYear);
            }


            var orders = await query
                .Select(o => new GetOrderByOrderIdDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = (DateTime)o.OrderDate,
                    TotalMoney = (double)o.TotalMoney,
                    Status = (bool)o.Status,
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
                        }).ToList(),
                    Customers = o.PointLists
                        .Where(pl => pl.Acc != null)
                        .Select(pl => new GetAccountDTO
                        {
                            AccId = pl.Acc.AccId,
                            FullName = pl.Acc.FullName,
                            Email = pl.Acc.Email,
                            Phone = pl.Acc.Phone
                        }).ToList(),
                    DiscountId = o.Code != null ? o.Code.CodeId : null,
                    DiscountValue = o.Code != null ? o.Code.DiscountValue : null,
                    PointIds = o.PointLists != null ? o.PointLists.Select(pl => pl.PointId).ToList() : new List<int>(),
                    PointNumbers = o.PointLists != null ? o.PointLists.Select(pl => (double?)pl.NumberPonit).ToList() : new List<double?>()
                })
                .ToListAsync();


            var totalRevenue = orders.Sum(o => o.TotalMoney);

            return (orders, (decimal)totalRevenue);
        }


        public int CountOrders()
        {
            return _context.Orders.Count(o => o.Status == true);
        }

    }
}


