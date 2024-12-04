//using Xunit;
//using Moq;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Linq;
//using SRMMS.Controllers; // Thay bằng namespace thực tế
//using SRMMS.Models;      // Thay bằng namespace chứa DbContext và Models
//using Microsoft.AspNetCore.Mvc;


//public class ProductControllerTests
//{
//    private Mock<SRMMSContext> _mockContext;

//    public ProductControllerTests()
//    {
//        _mockContext = new Mock<SRMMSContext>();
//    }
//    private Mock<DbSet<Product>> CreateMockDbSet(List<Product> data)
//    {
//        var queryable = data.AsQueryable();
//        var mockSet = new Mock<DbSet<Product>>();

//        mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(queryable.Provider);
//        mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(queryable.Expression);
//        mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
//        mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

//        mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
//               .Returns<object[]>(ids => Task.FromResult(data.FirstOrDefault(d => d.ProId == (int)ids[0])));

//        return mockSet;
//    }

//    [Fact]
//    public async Task DeleteProduct_ProductNotFound_ReturnsOkWithMessage()
//    {
//        // Arrange
//        var productId = 100; // Không tồn tại
//        var mockProducts = CreateMockDbSet(new List<Product>()); // Danh sách rỗng

//        _mockContext.Setup(m => m.Products).Returns(mockProducts.Object);

//        var controller = new ProductController(_mockContext.Object);

//        // Act
//        var result = await controller.DeleteProduct(productId);

//        // Assert
//        var okResult = Assert.IsType<OkObjectResult>(result);
//        var response = Assert.IsType<dynamic>(okResult.Value);
//        Assert.Equal("Product not found.", response.Message);
//    }


//    [Fact]
//    public async Task DeleteProduct_ProductWithoutRelations_DisablesProductStatus()
//    {
//        // Arrange
//        var productId = 1;
//        var product = new Product { ProId = productId, ProStatus = true };

//        _mockContext.Setup(m => m.Products.FindAsync(productId))
//                    .ReturnsAsync(product);

//        var controller = new ProductController(_mockContext.Object);

//        // Act
//        var result = await controller.DeleteProduct(productId);

//        // Assert
//        var okResult = Assert.IsType<OkObjectResult>(result);
//        var response = Assert.IsType<dynamic>(okResult.Value);
//        Assert.Equal("Trạng thái sản phẩm đã được vô hiệu hóa thành công.", response.Message);
//        Assert.False(product.ProStatus); // Kiểm tra trạng thái sản phẩm
//    }

//    [Fact]
//    public async Task DeleteProduct_ProductWithComboDetails_RemovesComboDetails()
//    {
//        // Arrange
//        var productId = 2;
//        var product = new Product { ProId = productId, ProStatus = true };

//        var comboDetails = new List<ComboDetail>
//        {
//            new ComboDetail {ComboId = 1, ProId = productId }
//        };

//        _mockContext.Setup(m => m.Products.FindAsync(productId))
//                    .ReturnsAsync(product);

//        _mockContext.Setup(m => m.ComboDetails.Where(cd => cd.ProId == productId))
//                    .Returns(comboDetails.AsQueryable());

//        var controller = new ProductController(_mockContext.Object);

//        // Act
//        var result = await controller.DeleteProduct(productId);

//        // Assert
//        var okResult = Assert.IsType<OkObjectResult>(result);
//        var response = Assert.IsType<dynamic>(okResult.Value);
//        Assert.Equal("Trạng thái sản phẩm đã được vô hiệu hóa thành công.", response.Message);

//        // Kiểm tra ComboDetails đã bị xóa
//        _mockContext.Verify(m => m.ComboDetails.RemoveRange(comboDetails), Times.Once);
//    }

//    [Fact]
//    public async Task DeleteProduct_ProductWithOrderStatus4_DisablesComboAndProduct()
//    {
//        // Arrange
//        var productId = 4;
//        var product = new Product { ProId = productId, ProStatus = true };

//        var orderDetails = new List<OrderDetail>
//        {
//            new OrderDetail
//            {
//                OrderDetailId = 1,
//                ProId = productId,
//                Order = new Order { OrderId = 2, StatusId = 4 }
//            }
//        };

//        var comboDetail = new ComboDetail { ComboId = 1, ProId = productId };
//        var combo = new Combo { ComboId = 1, ComboName = "Combo A", ComboStatus = true };

//        _mockContext.Setup(m => m.Products.FindAsync(productId))
//                    .ReturnsAsync(product);

//        _mockContext.Setup(m => m.OrderDetails.Include(o => o.Order).Where(od => od.ProId == productId))
//                    .Returns(orderDetails.AsQueryable());

//        _mockContext.Setup(m => m.ComboDetails.FirstOrDefault(cd => cd.ProId == productId))
//                    .Returns(comboDetail);

//        _mockContext.Setup(m => m.Combos.FindAsync(comboDetail.ComboId))
//                    .ReturnsAsync(combo);

//        var controller = new ProductController(_mockContext.Object);

//        // Act
//        var result = await controller.DeleteProduct(productId);

//        // Assert
//        var okResult = Assert.IsType<OkObjectResult>(result);
//        var response = Assert.IsType<dynamic>(okResult.Value);
//        Assert.Equal("Trạng thái sản phẩm đã được vô hiệu hóa thành công.", response.Message);

//        // Kiểm tra trạng thái Combo và Product đã bị vô hiệu hóa
//        Assert.False(product.ProStatus);
//        Assert.False(combo.ComboStatus);
//    }

//    [Fact]
//    public async Task DeleteProduct_DatabaseError_RollsBackTransaction()
//    {
//        // Arrange
//        var productId = 5;
//        _mockContext.Setup(m => m.Products.FindAsync(productId))
//                    .ThrowsAsync(new DbUpdateException("Database error."));

//        var controller = new ProductController(_mockContext.Object);

//        // Act
//        var result = await controller.DeleteProduct(productId);

//        // Assert
//        var statusCodeResult = Assert.IsType<ObjectResult>(result);
//        Assert.Equal(500, statusCodeResult.StatusCode);

//        var response = Assert.IsType<dynamic>(statusCodeResult.Value);
//        Assert.Equal("An error occurred.", response.Message);
//    }
//}
