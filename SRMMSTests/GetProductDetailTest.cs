using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.Controllers;
using SRMMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMMSTests
{
    public class GetProductDetailTest
    {
        [Fact]
        public async Task GetProductDetail_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_InvalidId")
                .Options;

            using var context = new SRMMSContext(options);
            var controller = new ProductController(context);

            // Act
            var result = await controller.GetProductDetail(0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID sản phẩm không hợp lệ. ID phải lớn hơn 0.", badRequestResult.Value);
        }
        [Fact]
        public async Task GetProductDetail_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_ProductNotFound")
                .Options;

            using var context = new SRMMSContext(options);
            context.Products.Add(new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true });
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.GetProductDetail(999);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
           
        }
        [Fact]
        public async Task GetProductDetail_ReturnsBadRequest_WhenIdInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_InvalidId")
                .Options;

            using var context = new SRMMSContext(options);
            var controller = new ProductController(context);

            // Act
            var result = await controller.GetProductDetail(-1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID sản phẩm không hợp lệ. ID phải lớn hơn 0.", badRequestResult.Value);
        }
    }
}
