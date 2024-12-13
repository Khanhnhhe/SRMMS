using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Query.Internal;
using SRMMS.Controllers;
using SRMMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRMMS.Controllers;
using SRMMS.DTOs;
namespace SRMMSTests
{
    public class SearchProductNameTest
    {
        [Fact]
        public async Task SearchByProductName_ReturnsBadRequest_WhenProductNameIsEmpty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_EmptyProductName")
                .Options;

            using var context = new SRMMSContext(options);
            context.Products.AddRange(
                new Product { ProId = 1, ProName = "Pizza", ProPrice = 10000, ProCalories = "250", ProStatus = true, CatId = 1 },
                new Product { ProId = 2, ProName = "Burger", ProPrice = 5000, ProCalories = "300", ProStatus = true, CatId = 1 }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.SearchByProductName(productName: "", pageNumber: 1, pageSize: 10);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = badRequestResult.Value;

            // Kiểm tra thông báo lỗi
            Assert.Equal("Tên sản phẩm không thể trống.", errorMessage);
        }
    }
}
