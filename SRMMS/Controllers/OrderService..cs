using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using CloudinaryDotNet;
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

            if (table.StatusId == 2)
            {
                var existingOrder = await _context.Orders
                .Where(o => o.TableId == orderDto.TableId && o.StatusId != 4) 
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync();


                if (existingOrder != null && existingOrder.StatusId < 4)
                {

                    if (existingOrder.StatusId != 1)
                    {
                        existingOrder.StatusId = 1; 
                    }

                    if (existingOrder.StatusId == 3)
                    {
                        
                        var status = await _context.StatusOrders.FirstOrDefaultAsync(s => s.StatusId == 1);
                        if (status == null)
                        {
                            throw new Exception("Trạng thái đơn hàng không hợp lệ.");
                        }

                        existingOrder.StatusId = status.StatusId; 
                    }

                    await AddOrderDetails(existingOrder, orderDto);

                    
                    existingOrder.TotalMoney += orderDto.TotalMoney;

                   
                    await _context.SaveChangesAsync();

                   
                    await _orderHubContext.Clients.All.SendAsync("ReceiveOrder", existingOrder);

                   
                    return existingOrder.OrderId;
                }
                else if(existingOrder == null ||  existingOrder.StatusId == 4)
                {
                    
                    var status = await _context.StatusOrders.FirstOrDefaultAsync(s => s.StatusId == 1); 
                    if (status == null)
                    {
                        throw new Exception("Trạng thái đơn hàng không hợp lệ.");
                    }

                    var newOrder = new Order
                    {
                        TableId = orderDto.TableId,
                        TotalMoney = orderDto.TotalMoney,
                        StatusId = status.StatusId, 
                        OrderDetails = new List<OrderDetail>()
                    };

                    
                    await AddOrderDetails(newOrder, orderDto);

                   
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

                   
                    await _orderHubContext.Clients.All.SendAsync("ReceiveOrder", newOrder);

                    
                    return newOrder.OrderId;
                }
            }

            throw new Exception("Bàn không có trạng thái 'Đang sử dụng'.");
        }


        private async Task AddOrderDetails(Order order, OrderDTO orderDto)
        {
          
            bool hasCombo = false;
            bool hasProduct = false;

           
            if (orderDto.ComboDetails != null && orderDto.ComboDetails.Any())
            {
                foreach (var comboDetail in orderDto.ComboDetails)
                {
                    var combo = await _context.Combos.FirstOrDefaultAsync(c => c.ComboId == comboDetail.ComboId);

                    if (combo == null)
                    {
                        throw new Exception($"Combo với ID {comboDetail.ComboId} không tồn tại.");
                    }

                    if (combo.ComboStatus == false)
                    {
                        throw new Exception($"Combo với ID {comboDetail.ComboId} đã bị vô hiệu hóa.");
                    }

                    if (comboDetail.Quantity <= 0)
                    {
                        throw new Exception($"Số lượng combo với ID {comboDetail.ComboId} phải lớn hơn 0.");
                    }

                   
                    var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ComboId == comboDetail.ComboId);
                    if (orderDetail != null)
                    {
                       
                        orderDetail.Quantiity += comboDetail.Quantity;
                    }
                    else
                    {
                        
                        orderDetail = new OrderDetail
                        {
                            ComboId = comboDetail.ComboId,
                            Quantiity = comboDetail.Quantity,
                            Price = comboDetail.Price
                        };
                        order.OrderDetails.Add(orderDetail);
                    }
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
                        throw new Exception($"Sản phẩm với ID {productDetail.ProId} đã bị vô hiệu hóa.");
                    }

                    if (productDetail.Quantity <= 0)
                    {
                        throw new Exception($"Số lượng sản phẩm với ID {productDetail.ProId} phải lớn hơn 0.");
                    }

                    
                    var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProId == productDetail.ProId);
                    if (orderDetail != null)
                    {
                        
                        orderDetail.Quantiity += productDetail.Quantity;
                    }
                    else
                    {
                       
                        orderDetail = new OrderDetail
                        {
                            ProId = productDetail.ProId,
                            Quantiity = productDetail.Quantity,
                            Price = productDetail.Price
                        };
                        order.OrderDetails.Add(orderDetail);
                    }
                }
                hasProduct = true;
            }

           
            if (!hasCombo && !hasProduct)
            {
                throw new Exception("Đơn hàng phải có ít nhất một sản phẩm hoặc combo.");
            }
        }

        //staff xác nhận và update 
        public async Task<int> ConfirmOrder(int tableId, ComfirmOrderDTO orderDto)
        {
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == tableId);

            if (table == null)
            {
                throw new Exception("Không tìm thấy bàn này.");
            }

            if (table.StatusId != 2)
            {
                throw new Exception("Bàn không ở trạng thái chờ xử lý.");
            }

            var existingOrder = await _context.Orders
                .Where(o => o.TableId == tableId && o.StatusId == 1)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync();

            if (existingOrder == null)
            {
                throw new Exception("Không tìm thấy đơn hàng cho bàn này hoặc đơn hàng không ở trạng thái chờ xử lý.");
            }

            // Lấy danh sách các ID sản phẩm và combo từ dữ liệu mới
            var newProductIds = orderDto.ProductDetails.Select(p => p.ProId).ToList();
            var newComboIds = orderDto.ComboDetails.Select(c => c.ComboId).ToList();

            // Xóa các sản phẩm cũ không còn trong danh sách mới
            var removedProductDetails = existingOrder.OrderDetails
                .Where(od => od.ProId != null && !newProductIds.Contains(od.ProId.Value))
                .ToList();
            foreach (var detail in removedProductDetails)
            {
                existingOrder.OrderDetails.Remove(detail);
            }

            // Xóa các combo cũ không còn trong danh sách mới
            var removedComboDetails = existingOrder.OrderDetails
                .Where(od => od.ComboId != null && !newComboIds.Contains(od.ComboId.Value))
                .ToList();
            foreach (var detail in removedComboDetails)
            {
                existingOrder.OrderDetails.Remove(detail);
            }

            // Thêm hoặc cập nhật sản phẩm
            foreach (var productDto in orderDto.ProductDetails)
            {
                var existingProductDetail = existingOrder.OrderDetails
                    .FirstOrDefault(od => od.ProId == productDto.ProId);

                if (existingProductDetail != null)
                {
                    existingProductDetail.Quantiity = productDto.Quantity;
                }
                else
                {
                    var newProductDetail = new OrderDetail
                    {
                        ProId = productDto.ProId,
                        Quantiity = productDto.Quantity,
                        Price = productDto.Price,
                        OrderId = existingOrder.OrderId
                    };
                    existingOrder.OrderDetails.Add(newProductDetail);
                }
            }

            // Thêm hoặc cập nhật combo
            foreach (var comboDto in orderDto.ComboDetails)
            {
                var existingComboDetail = existingOrder.OrderDetails
                    .FirstOrDefault(od => od.ComboId == comboDto.ComboId);

                if (existingComboDetail != null)
                {
                    existingComboDetail.Quantiity = comboDto.Quantity;
                }
                else
                {
                    var newComboDetail = new OrderDetail
                    {
                        ComboId = comboDto.ComboId,
                        Quantiity = comboDto.Quantity,
                        Price = comboDto.Price,
                        OrderId = existingOrder.OrderId
                    };
                    existingOrder.OrderDetails.Add(newComboDetail);
                }
            }

            // Cập nhật tổng tiền
            existingOrder.TotalMoney = (decimal)orderDto.TotalMoney;

            // Cập nhật trạng thái đơn hàng
            existingOrder.StatusId = 2;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Thông báo qua SignalR
            await _orderHubContext.Clients.All.SendAsync("ReceiveOrderConfirmation", existingOrder);

            return existingOrder.OrderId;
        }


        // bếp hoàn thành

        public async Task<int> ChangeOrderStatusToComplete(int orderId)
        {
            
            var existingOrder = await _context.Orders
                .Where(o => o.OrderId == orderId)
                .Include(o => o.Table) 
                .FirstOrDefaultAsync();

            if (existingOrder == null)
            {
                throw new Exception("Đơn hàng không tồn tại.");
            }

           
            if (existingOrder.StatusId != 2) 
            {
                throw new Exception("Đơn hàng không thể chuyển trạng thái vì không phải trạng thái 'Đang sử dụng'.");
            }

            
            existingOrder.StatusId = 3;

            
            await _context.SaveChangesAsync();

           
            await _orderHubContext.Clients.All.SendAsync("ReceiveOrder", existingOrder);

            return existingOrder.OrderId;
        }


        public (List<GetOrderByTableNameDTO> Orders, int TotalOrders) GetOrdersStatus23(
     int pageNumber = 1,
     int pageSize = 10,
     string? tableName = null,
     string? fromDate = null,
     string? toDate = null)
        {
            var query = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Pro)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                .Where(o => o.StatusId == 2 || o.StatusId == 3) 
                .AsQueryable();

            
            if (!string.IsNullOrEmpty(tableName))
            {
                string normalizedTableName = tableName.Trim().Replace(" ", "").ToLower();
                var tableExists = _context.Tables
                    .Any(t => t.TableName.Replace(" ", "").ToLower().Contains(normalizedTableName));

                if (!tableExists)
                {
                    throw new Exception($"Bàn với tên '{tableName}' không tồn tại");
                }

                query = query.Where(o => o.Table.TableName.Replace(" ", "").ToLower().Contains(normalizedTableName));
            }

            
            if (!string.IsNullOrEmpty(fromDate))
            {
                if (DateTime.TryParse(fromDate, out var parsedFromDate))
                {
                    query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= parsedFromDate);
                }
                else
                {
                    throw new Exception($"Định dạng fromDate không hợp lệ: '{fromDate}'. Định dạng dự kiến ​​là yyyy-MM-dd.");
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
                    throw new Exception($"Định dạng toDate không hợp lệ: '{toDate}'. Định dạng dự kiến ​​là yyyy-MM-dd.");
                }
            }

           
            var totalOrders = query.Count();

            
            var orders = query
                .Select(o => new GetOrderByTableNameDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), 
                    TotalMoney = o.TotalMoney, 
                    Status = o.StatusId,
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




        public (List<GetOrderByOrderIdDTO> Orders, int TotalOrders) GetOrders(
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
                .Where(o => o.StatusId == 4)
                .AsQueryable();


            if (!string.IsNullOrEmpty(tableName))
            {
                string normalizedTableName = tableName.Trim().Replace(" ", "").ToLower();
                var tableExists = _context.Tables
                    .Any(t => t.TableName.Replace(" ", "").Contains(normalizedTableName));

                if (!tableExists)
                {
                    throw new Exception($"Bàn với tên '{tableName}' không tồn taị");
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
                    throw new Exception($"Định dạng fromDate không hợp lệ: '{fromDate}'. Định dạng dự kiến ​​là yyyy-MM-dd.");
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
                    throw new Exception($"Định dạng fromDate không hợp lệ: '{toDate}'. Định dạng dự kiến ​​là  yyyy-MM-dd.");
                }
            }
            var totalOrders = query.Count();


            var orders = query
                .Select(o => new GetOrderByOrderIdDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalMoney = (double)o.TotalMoney,
                    Status = o.StatusId,
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
                    Customers = o.PointLists != null
            ? o.PointLists
                .Where(pl => pl.Acc != null)
                .Select(pl => new GetAccountDTO
                {
                    AccId = pl.Acc.AccId,
                    FullName = pl.Acc.FullName,
                    Email = pl.Acc.Email,
                    Phone = pl.Acc.Phone
                }).ToList()
            : new List<GetAccountDTO>(),
                    DiscountId = o.Code != null ? o.Code.CodeId : null,
                    DiscountValue = o.Code != null ? o.Code.DiscountValue : null,
                    PointIds = o.PointLists != null ? o.PointLists.Select(pl => pl.PointId).ToList() : new List<int>(),
                    PointNumbers = o.PointLists != null
            ? o.PointLists.Select(pl => (double?)pl.NumberPonit).ToList()
            : new List<double?>()
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (orders, totalOrders);
        }

        public (List<GetOrderByOrderIdDTO> Orders, int TotalOrders) GetOrdersAllStatus(
    int pageNumber = 1,
    int pageSize = 10,
    string? tableName = null,
    string? fromDate = null,
    string? toDate = null,
    int? statusId = null) 
        {
            var query = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Pro)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Combo)
                .AsQueryable();

          
            if (statusId.HasValue)
            {
                query = query.Where(o => o.StatusId == statusId.Value);
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                string normalizedTableName = tableName.Trim().Replace(" ", "").ToLower();
                var tableExists = _context.Tables
                    .Any(t => t.TableName.Replace(" ", "").Contains(normalizedTableName));

                if (!tableExists)
                {
                    throw new Exception($"Bàn với tên '{tableName}' không tồn tại");
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
                    throw new Exception($"Định dạng fromDate không hợp lệ: '{fromDate}'. Định dạng dự kiến ​​là yyyy-MM-dd.");
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
                    throw new Exception($"Định dạng toDate không hợp lệ: '{toDate}'. Định dạng dự kiến ​​là yyyy-MM-dd.");
                }
            }

            var totalOrders = query.Count();

            var orders = query
                .Select(o => new GetOrderByOrderIdDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalMoney = (double)o.TotalMoney,
                    Status = o.StatusId,
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
                    Customers = o.PointLists != null
                        ? o.PointLists
                            .Where(pl => pl.Acc != null)
                            .Select(pl => new GetAccountDTO
                            {
                                AccId = pl.Acc.AccId,
                                FullName = pl.Acc.FullName,
                                Email = pl.Acc.Email,
                                Phone = pl.Acc.Phone
                            }).ToList()
                        : new List<GetAccountDTO>(),
                    DiscountId = o.Code != null ? o.Code.CodeId : null,
                    DiscountValue = o.Code != null ? o.Code.DiscountValue : null,
                    PointIds = o.PointLists != null ? o.PointLists.Select(pl => pl.PointId).ToList() : new List<int>(),
                    PointNumbers = o.PointLists != null
                        ? o.PointLists.Select(pl => (double?)pl.NumberPonit).ToList()
                        : new List<double?>()
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
                throw new Exception($"Order với ID {orderId} không tồn tại.");
            }

            return new GetOrderByOrderIdDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate.HasValue ? (DateTime)order.OrderDate : null,
                TotalMoney = (double)order.TotalMoney,
                Status = order.StatusId, 
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

                
                Customers = order.PointLists?
                    .Where(pl => pl.Acc != null) 
                    .Select(pl => new GetAccountDTO
                    {
                        AccId = pl.Acc.AccId,
                        FullName = pl.Acc.FullName,
                        Email = pl.Acc.Email,
                        Phone = pl.Acc.Phone
                    }).ToList() ?? new List<GetAccountDTO>(),

                DiscountId = order.Code?.CodeId, 
                DiscountValue = order.Code?.DiscountValue, 
                PointIds = order.PointLists?.Select(pl => pl.PointId).ToList() ?? new List<int>(),
                PointNumbers = order.PointLists?.Select(pl => (double?)pl.NumberPonit).ToList() ?? new List<double?>()
            };
        }





        public List<GetOrderByTableNameDTO> GetOrdersByTable(int tableId)
        {
           
            var tableExists = _context.Tables.Any(t => t.TableId == tableId);
            if (!tableExists)
            {
                throw new Exception($"Bàn với ID '{tableId}' không tồn tại");
            }

            
            var query = _context.Orders
                .Where(o => o.TableId == tableId) 
                .Where(o => o.StatusId == 1 || o.StatusId == 2 || o.StatusId == 3) 
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Pro)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                .AsQueryable();

          
            var orders = query
                .Select(o => new GetOrderByTableNameDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.HasValue
                        ? o.OrderDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : null, 
                    TotalMoney = o.TotalMoney,
                    Status = o.StatusId,
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
                .ToList();

            return orders;
        }



        public List<GetOrderByTableNameDTO> SearchOrdersByTableName(string tableName, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new Exception("Tên bàn không được để trống.");
            }

            
            string normalizedTableName = tableName.Trim().Replace(" ", "").ToLower();

            
            var tablesExist = _context.Tables
                .Where(t => t.TableName != null && t.TableName.Replace(" ", "").ToLower().Contains(normalizedTableName))
                .Any();

            if (!tablesExist)
            {
                throw new Exception($"Không tìm thấy bàn với tên chứa '{tableName}'.");
            }

            
            var query = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Pro)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                .Where(o => o.Table.TableName != null && o.Table.TableName.Replace(" ", "").ToLower().Contains(normalizedTableName)) 
                .AsQueryable();

         
            var orders = query
                .Select(o => new GetOrderByTableNameDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.HasValue ? o.OrderDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null, // Đảm bảo ngày tháng đúng định dạng
                    TotalMoney = o.TotalMoney,
                    Status = o.StatusId,
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

        public async Task<OrderCompleteDTO> CompleteOrder(int orderId, CompleteOrderDTO orderComplete)
        {
            
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new Exception("Không tìm thấy Order.");
            }

            if (order.StatusId == 4)
            {
                throw new Exception("Đơn hàng này đã hoàn tất và không thể sửa đổi.");
            }

            if (order.StatusId != 3)
            {
                throw new Exception("Đơn hàng này không thể thanh toán vì có món chưa được hoàn thành");
            }

            double? discountValue = null;

            
            if (orderComplete.discountId.HasValue)
            {
                var discount = await _context.DiscountCodes.FirstOrDefaultAsync(d => d.CodeId == orderComplete.discountId.Value);

                if (discount == null)
                {
                    throw new Exception("Không tìm thấy mã giảm giá.");
                }

                if (!(discount.Status ?? false) || discount.StartDate > DateTime.Now ||
                    (discount.EndDate.HasValue && discount.EndDate.Value < DateTime.Now))
                {
                    throw new Exception("Mã giảm giá không hợp lệ hoặc đã hết hạn.");
                }

                discountValue = discount.DiscountValue;

                if (discount.DiscountType == (int)DiscountType.Percentage)
                {

                    if (orderComplete.totalMoney.HasValue)
                    {
                        var discountAmount = (double)orderComplete.totalMoney.Value * (discountValue.Value / 100);

                        orderComplete.totalMoney -= (decimal)discountAmount;
                    }
                }
                else if (discount.DiscountType == (int)DiscountType.Fixed)
                {

                    if (orderComplete.totalMoney.HasValue)
                    {
                        if (discountValue >= (double)orderComplete.totalMoney.Value)
                        {
                            orderComplete.totalMoney = 0;
                        }
                        else
                        {
                            orderComplete.totalMoney -= (decimal)discountValue;
                        }
                    }
                }
            }

            if (orderComplete.totalMoney.HasValue)
            {
                order.TotalMoney = orderComplete.totalMoney.Value;
            }


            if (orderComplete.accId.HasValue && orderComplete.usedPoints.HasValue && orderComplete.usedPoints > 0)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccId == orderComplete.accId.Value);

                if (account == null)
                {
                    throw new Exception("Không tìm thấy tài khoản khách hàng.");
                }

                var conversionSettings = await _context.ConversionPoints.FirstOrDefaultAsync();

                if (conversionSettings == null)
                {
                    throw new Exception("Chưa có tỷ lệ quy đổi điểm.");
                }

                var pointsToMoney = orderComplete.usedPoints.Value * conversionSettings.PointToMoneyRate;

                var totalPoints = await _context.PointLists
                    .Where(p => p.AccId == orderComplete.accId.Value)
                    .SumAsync(p => p.NumberPonit);

                if (totalPoints < orderComplete.usedPoints.Value)
                {
                    throw new Exception("Số điểm sử dụng vượt quá số điểm hiện có.");
                }

                if (order.TotalMoney < pointsToMoney)
                {
                    order.TotalMoney = 0;
                }
                else
                {
                    order.TotalMoney -= pointsToMoney;
                }

                var pointRecord = await _context.PointLists
                    .Where(p => p.AccId == orderComplete.accId.Value)
                    .FirstOrDefaultAsync();

                if (pointRecord != null)
                {
                    pointRecord.NumberPonit -= orderComplete.usedPoints.Value;
                    _context.PointLists.Update(pointRecord);
                }

               
            }

            if (orderComplete.accId.HasValue)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccId == orderComplete.accId.Value);

                if (account == null)
                {
                    throw new Exception("Không tìm thấy tài khoản khách hàng.");
                }

                var conversionSettings = await _context.ConversionPoints.FirstOrDefaultAsync();
                if (conversionSettings != null)
                {
                    var pointsEarned = (long)(order.TotalMoney / conversionSettings.MoneyToPointRate); 
                    var pointRecord = await _context.PointLists
                        .Where(p => p.AccId == orderComplete.accId.Value)
                        .FirstOrDefaultAsync();

                    if (pointRecord != null)
                    {
                        pointRecord.NumberPonit += pointsEarned;
                        _context.PointLists.Update(pointRecord);
                    }
                    else
                    {
                        _context.PointLists.Add(new PointList
                        {
                            AccId = orderComplete.accId.Value,
                            NumberPonit = pointsEarned
                        });
                    }
                }
            }

            order.OrderDate = DateTime.Now;
            order.CodeId = orderComplete.discountId;

            if (order.Table != null)
            {
                order.Table.StatusId = 1;
                order.Table.BookingId = null;
            }

            order.StatusId = 4; 

            await _context.SaveChangesAsync();

            return new OrderCompleteDTO
            {
                OrderId = order.OrderId,
                TotalMoney = order.TotalMoney,
                TableId = order.TableId,
                OrderDate = order.OrderDate,
                Status = order.StatusId,
                DiscountId = order.CodeId,
                DiscountValue = discountValue
            };
        }



        public async Task<(List<GetOrderByOrderIdDTO> Orders, decimal TotalRevenue)> CalculateTotalRevenue(int? weekNumber = null, int? month = null, int? year = null)
        {
            var query = _context.Orders
                                .Where(o => o.StatusId == 4);


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
                        startOfWeek = firstDayOfMonth.AddDays(21); // Bắt đầu tuần 4 từ ngày 22
                        endOfWeek = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59); // Kết thúc tuần 4 vào cuối ngày cuối tháng
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
                    Status = o.Status.StatusId,
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
            return _context.Orders.Count(o => o.StatusId == 4);
        }

    }
}


