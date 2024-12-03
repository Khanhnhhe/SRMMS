using Microsoft.AspNetCore.Mvc;
using SRMMS.Controllers;
using Moq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;
using System;
using System.Text.Json;

namespace SRMMSTests
{
    public class OrderControllerTests
    {
        [Fact]
        public async Task AddOrder_ShouldThrowException_WhenOrderDetailsEmpty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_AddOrder_EmptyOrderDetails")
                .Options;

            using var context = new SRMMSContext(options);
            context.Tables.Add(new Table { TableId = 1, StatusId = 2 }); // Bàn đang sử dụng
            context.StatusOrders.Add(new StatusOrder { StatusId = 1, StatusName = "Đang xử lý" });
            context.SaveChanges();

            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            var service = new OrderService(context, mockHubContext.Object);
            var controller = new OrderController(service);

            var orderDto = new OrderDTO
            {
                TableId = 1,
                TotalMoney = 100000,
                OrderDetails = new List<OrderDetailDTO>() // OrderDetails rỗng
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => controller.AddOrder(orderDto));
            Assert.Equal("Đơn hàng phải có ít nhất một sản phẩm hoặc combo.", exception.Message);
        }
        [Fact]
        public async Task AddOrder_ShouldThrowException_WhenTableIdNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_TableNotFound")
                .Options;

            using var context = new SRMMSContext(options);

            // Lưu ý: Không thêm bất kỳ bảng nào vào `context.Tables`
            context.StatusOrders.Add(new StatusOrder { StatusId = 1, StatusName = "Đang xử lý" });
            context.SaveChanges();

            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            var service = new OrderService(context, mockHubContext.Object);
            var controller = new OrderController(service);

            var orderDto = new OrderDTO
            {
                TableId = 999, // TableId không tồn tại
                TotalMoney = 100000,
                OrderDetails = new List<OrderDetailDTO>
        {
            new OrderDetailDTO { ProId = 1, Quantity = 1, Price = 50000 }
        }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => controller.AddOrder(orderDto));
            Assert.Equal("TableId không tồn tại.", exception.Message);
        }
        [Fact]
        public async Task AddOrder_ShouldThrowException_WhenComboIsDisabled()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_ComboDisabled")
                .Options;

            using var context = new SRMMSContext(options);

            // Thêm dữ liệu cần thiết vào cơ sở dữ liệu
            context.Tables.Add(new Table { TableId = 1, StatusId = 2 }); // Table đang sử dụng
            context.Combos.Add(new Combo { ComboId = 1, ComboStatus = false }); // Combo bị vô hiệu hóa
            context.StatusOrders.Add(new StatusOrder { StatusId = 1, StatusName = "Đang xử lý" });
            context.SaveChanges();

            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            var service = new OrderService(context, mockHubContext.Object);
            var controller = new OrderController(service);

            var orderDto = new OrderDTO
            {
                TableId = 1,
                TotalMoney = 500,
                OrderDetails = new List<OrderDetailDTO>
        {
            new OrderDetailDTO { ProId = 1, Quantity = 2, Price = 500 }
        }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => controller.AddOrder(orderDto));
            Assert.Equal("Combo with ID 1 has been disabled.", exception.Message);
        }
        [Fact]
        public async Task AddOrder_ShouldThrowException_WhenTableIsNotInUse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_TableNotInUse")
                .Options;

            using var context = new SRMMSContext(options);

            // Thêm dữ liệu cần thiết vào cơ sở dữ liệu
            context.Tables.Add(new Table { TableId = 1, StatusId = 1 }); // Bàn ở trạng thái không sử dụng
            context.StatusOrders.Add(new StatusOrder { StatusId = 1, StatusName = "Đang xử lý" });
            context.SaveChanges();

            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            var service = new OrderService(context, mockHubContext.Object);
            var controller = new OrderController(service);

            var orderDto = new OrderDTO
            {
                TableId = 1,
                TotalMoney = 500,
                OrderDetails = new List<OrderDetailDTO>
        {
            new OrderDetailDTO { ProId = 1, Quantity = 2, Price = 500 }
        }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => controller.AddOrder(orderDto));
            Assert.Equal("Bàn không có trạng thái 'Đang sử dụng'.", exception.Message);
        }

        [Fact]
        public async Task AddOrder_ShouldThrowException_WhenOrderStatusIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_InvalidOrderStatus")
                .Options;

            using var context = new SRMMSContext(options);

            // Thêm dữ liệu cần thiết vào cơ sở dữ liệu
            context.Tables.Add(new Table { TableId = 1, StatusId = 2 }); // Bàn ở trạng thái "Đang sử dụng"
            context.SaveChanges();

            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            var service = new OrderService(context, mockHubContext.Object);
            var controller = new OrderController(service);

            var orderDto = new OrderDTO
            {
                TableId = 1,
                TotalMoney = 500,
                OrderDetails = new List<OrderDetailDTO>
        {
            new OrderDetailDTO { ProId = 1, Quantity = 2, Price = 500 }
        }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => controller.AddOrder(orderDto));
            Assert.Equal("Trạng thái đơn hàng không hợp lệ.", exception.Message);
        }
        [Fact]
        public async Task AddOrder_ShouldThrowException_WhenComboQuantityIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_InvalidComboQuantity")
                .Options;

            using var context = new SRMMSContext(options);

            // Thêm dữ liệu cần thiết vào cơ sở dữ liệu
            context.Tables.Add(new Table { TableId = 1, StatusId = 2 }); // Bàn ở trạng thái "Đang sử dụng"
            context.StatusOrders.Add(new StatusOrder { StatusId = 1, StatusName = "Mới" }); // Trạng thái đơn hàng hợp lệ
            context.Combos.Add(new Combo { ComboId = 1, ComboStatus = true }); // Combo hợp lệ
            context.SaveChanges();

            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            var service = new OrderService(context, mockHubContext.Object);
            var controller = new OrderController(service);

            var orderDto = new OrderDTO
            {
                TableId = 1,
                TotalMoney = 500,
                OrderDetails = new List<OrderDetailDTO>
        {
            new OrderDetailDTO { ProId = 1, Quantity = 0, Price = 500 } // Số lượng không hợp lệ
        }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => controller.AddOrder(orderDto));
            Assert.Equal("Số lượng combo với ID 1 phải lớn hơn 0.", exception.Message);
        }
        [Fact]
        public async Task AddOrder_ShouldThrowException_WhenTotalMoneyIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_InvalidTotalMoney")
                .Options;

            using var context = new SRMMSContext(options);

            // Thêm dữ liệu cần thiết vào cơ sở dữ liệu
            context.Tables.Add(new Table { TableId = 1, StatusId = 2 }); // Bàn ở trạng thái "Đang sử dụng"
            context.StatusOrders.Add(new StatusOrder { StatusId = 1, StatusName = "1" }); // Trạng thái đơn hàng hợp lệ
            context.SaveChanges();

            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            var service = new OrderService(context, mockHubContext.Object);
            var controller = new OrderController(service);

            var orderDto = new OrderDTO
            {
                TableId = 1,
                TotalMoney = 0, // Tổng số tiền không hợp lệ
                OrderDetails = new List<OrderDetailDTO>
        {
            new OrderDetailDTO { ProId = 1, Quantity = 1, Price = 500 }
        }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => controller.AddOrder(orderDto));
            Assert.Equal("Tổng số tiền phải lớn hơn 0.", exception.Message);
        }
        [Fact]
        public async Task AddOrder_ShouldReturnOrderId_WhenOrderIsSuccessful()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_ValidOrder")
                .Options;

            using var context = new SRMMSContext(options);

            // Thêm dữ liệu cần thiết vào cơ sở dữ liệu (nếu có)
            context.Tables.Add(new Table { TableId = 7, StatusId = 2 }); // Bàn hợp lệ (Đang sử dụng)
            context.StatusOrders.Add(new StatusOrder { StatusId = 1, StatusName = "Mới" }); // Trạng thái hợp lệ
            context.Products.Add(new Product { ProId = 1, ProName = "Product 1", ProPrice = 10 });
            context.SaveChanges();

            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            var service = new OrderService(context, mockHubContext.Object);
            var controller = new OrderController(service);

            var orderDto = new OrderDTO
            {
                OrderId = 0, // Tạo mới
                TableId = 7,  // Bàn hợp lệ
                TotalMoney = 1100, // Tổng tiền hợp lệ
                Status = 1,  // Trạng thái hợp lệ (Mới)
                ComboDetails = new List<ComboDetailDTO>(), // Không có combo
                ProductDetails = new List<ProductDetailOrderDTO>
        {
            new ProductDetailOrderDTO { ProductId = 1, Quantity = 10, Price = 1110 } // Sản phẩm hợp lệ
        },
                OrderDetails = new List<OrderDetailDTO>
        {
            new OrderDetailDTO { ProId = 1, Quantity = 10, Price = 10 }
        }
            };

            // Act
            var result = await controller.AddOrder(orderDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<JsonElement>(okResult.Value);

            Assert.Contains("OrderId", response.ToString());  // Kiểm tra nếu OrderId có trong phản hồi
        }

    }
}