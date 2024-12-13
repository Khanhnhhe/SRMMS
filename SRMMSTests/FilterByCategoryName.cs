using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.Controllers;
using SRMMS.DTOs;
using SRMMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMMSTests
{
    public class FilterByCategoryName
    {
        private DbContextOptions<SRMMSContext> GetInMemoryDbOptions(string dbName)
        {
            return new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }
        [Fact]
        public async Task FilterByCategoryName_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase("TestDatabase_CategoryNotFound")
                .Options;

            using var context = new SRMMSContext(options);

            // Thêm một số danh mục (không có danh mục tên "abcxzy")
            context.Categories.Add(new Category { CatId = 1, CatName = "Đồ ăn" });
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.FilterByCategoryName("abcxzy");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
        }
        [Fact]
        public async Task FilterByCategoryName_ReturnsProducts_WhenCategoryExists()
        {
            // Arrange
            var options = GetInMemoryDbOptions("ValidCategoryDb");
            using var context = new SRMMSContext(options);
            context.Categories.Add(new Category { CatId = 1, CatName = "Hải sản" });
            context.Products.Add(new Product { ProId = 1, ProName = "Cua", CatId = 1, ProPrice = 100000 });
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.FilterByCategoryName("Hải sản");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<dynamic>(okResult.Value);
            Assert.Equal(1, value.TotalProducts);
            Assert.Single(value.Products);
            Assert.Equal("Cua", value.Products[0].ProductName);
        }

    }
}
