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
    public class ProductControllerTest
    {
        [Fact]
        public async Task GetAllProducts_ReturnsEmptyList_WhenMinPriceAndMaxPriceAreNegative()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_NegativePrice")
                .Options;

            using var context = new SRMMSContext(options);
            context.Products.AddRange(
                new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true },
                new Product { ProId = 2, ProName = "Pizza", ProPrice = 15000, CatId = 2, ProStatus = true },
                new Product { ProId = 3, ProName = "Trứng luộc", ProPrice = 4000, CatId = 1, ProStatus = true }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(minPrice: "-5000", maxPrice: "-10000");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var totalProducts = (int)response.GetType().GetProperty("TotalProducts").GetValue(response);
            var totalPages = (int)response.GetType().GetProperty("TotalPages").GetValue(response);
            var products = response.GetType().GetProperty("Products").GetValue(response) as List<ListProductDTO>;

            // Assertions
            Assert.Equal(0, totalProducts); // Expecting 0 products because of negative price range
            Assert.Equal(0, totalPages); // Expecting 0 pages since no products match the criteria
            Assert.Empty(products); // Products list should be empty
        }

        [Fact]
        public async Task GetAllProducts_ReturnsEmptyList_WhenCategoryIdIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_InvalidCategoryId")
                .Options;

            using var context = new SRMMSContext(options);
            context.Products.AddRange(
                new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true },
                new Product { ProId = 2, ProName = "Pizza", ProPrice = 15000, CatId = 2, ProStatus = true },
                new Product { ProId = 3, ProName = "Trứng luộc", ProPrice = 4000, CatId = 1, ProStatus = true }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(categoryId: -1); // Using an invalid categoryId (-1)

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var totalProducts = (int)response.GetType().GetProperty("TotalProducts").GetValue(response);
            var totalPages = (int)response.GetType().GetProperty("TotalPages").GetValue(response);
            var products = response.GetType().GetProperty("Products").GetValue(response) as List<ListProductDTO>;

            // Assertions
            Assert.Equal(0, totalProducts); // Expecting 0 products since the categoryId is invalid
            Assert.Equal(0, totalPages); // Expecting 0 pages since no products are found
            Assert.Empty(products); // Products list should be empty

            var message = response.GetType().GetProperty("Message")?.GetValue(response);
            Assert.Equal("Không trả về kết quả nào được tìm thấy với id -1.", message);
        }
        [Fact]
        public async Task GetAllProducts_ReturnsFilteredProducts_WhenNameAndPriceRangeAreProvided()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_PizzaFilter")
                .Options;

            using var context = new SRMMSContext(options);
            context.Products.AddRange(
                new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true },
                new Product { ProId = 2, ProName = "Pizza Margherita", ProPrice = 8000, CatId = 1, ProStatus = true },
                new Product { ProId = 3, ProName = "Pizza Pepperoni", ProPrice = 12000, CatId = 1, ProStatus = true },
                new Product { ProId = 4, ProName = "Sushi", ProPrice = 7000, CatId = 2, ProStatus = true }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(name: "Pizza", minPrice: "5000", maxPrice: "10000");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var totalProducts = (int)response.GetType().GetProperty("TotalProducts").GetValue(response);
            var totalPages = (int)response.GetType().GetProperty("TotalPages").GetValue(response);
            var products = response.GetType().GetProperty("Products").GetValue(response) as List<ListProductDTO>;

            // Assertions
            Assert.Equal(1, totalProducts); // Expecting 1 product matching criteria
            Assert.Equal(1, totalPages); // Expecting 1 page for the result
            Assert.Single(products); // Only 1 product should be returned

            var product = products.First();
            Assert.Equal("Pizza Margherita", product.ProductName);
            Assert.Equal(8000, product.Price);
    
        }
        [Fact]
        public async Task GetAllProducts_ReturnsEmptyList_WhenNameOrIdDoNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_NoProductFound")
                .Options;

            using var context = new SRMMSContext(options);
            context.Products.AddRange(
                new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true },
                new Product { ProId = 2, ProName = "Pizza", ProPrice = 15000, CatId = 2, ProStatus = true },
                new Product { ProId = 3, ProName = "Trứng luộc", ProPrice = 4000, CatId = 1, ProStatus = true }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(name: "Sushi", categoryId: 9999);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var totalProducts = (int)response.GetType().GetProperty("TotalProducts").GetValue(response);
            var totalPages = (int)response.GetType().GetProperty("TotalPages").GetValue(response);
            var products = response.GetType().GetProperty("Products").GetValue(response) as List<ListProductDTO>;

            // Assertions
            Assert.Equal(0, totalProducts); // No products found
            Assert.Equal(0, totalPages); // No pages since no products
            Assert.Empty(products); // Products list should be empty

            // Kiểm tra thông báo không có sản phẩm nào đáp ứng tiêu chí tìm kiếm (tùy vào cách API trả thông báo)
            var message = response.GetType().GetProperty("Message")?.GetValue(response);
            Assert.Equal("Không có sản phẩm nào đáp ứng tiêu chí tìm kiếm.", message);
        }
        [Fact]
        public async Task GetAllProducts_ReturnsBadRequest_WhenMinPriceIsNotANumber()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_InvalidMinPrice")
                .Options;

            using var context = new SRMMSContext(options);
            context.Products.AddRange(
                new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true },
                new Product { ProId = 2, ProName = "Pizza", ProPrice = 15000, CatId = 2, ProStatus = true },
                new Product { ProId = 3, ProName = "Trứng luộc", ProPrice = 4000, CatId = 1, ProStatus = true }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(minPrice: "abc", maxPrice: "10000");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = badRequestResult.Value;

            // Kiểm tra thông báo lỗi
            Assert.Equal("Giá tối thiểu không hợp lệ. Vui lòng nhập giá trị số.", errorMessage);
        }
        [Fact]
        public async Task GetAllProducts_ReturnsProductsFilteredByMaxPrice_WhenMaxPriceIsProvided()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SRMMSContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_MaxPriceFilter")
                .Options;

            using var context = new SRMMSContext(options);
            context.Products.AddRange(
                new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true },
                new Product { ProId = 2, ProName = "Pizza", ProPrice = 15000, CatId = 2, ProStatus = true },
                new Product { ProId = 3, ProName = "Trứng luộc", ProPrice = 4000, CatId = 1, ProStatus = true }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(maxPrice: "10000");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var totalProducts = (int)response.GetType().GetProperty("TotalProducts").GetValue(response);
            var products = response.GetType().GetProperty("Products").GetValue(response) as List<ListProductDTO>;

            // Assertions
            Assert.Equal(2, totalProducts); // Expecting 2 products with price <= 10000
            Assert.Equal(2, products.Count); // Expecting 2 products returned

            // Sắp xếp các sản phẩm theo giá (hoặc theo tên nếu muốn)
            var sortedProducts = products.OrderBy(p => p.Price).ToList();

            // Kiểm tra sản phẩm đầu tiên và cuối cùng sau khi sắp xếp
            var product1 = sortedProducts.First();
            var product2 = sortedProducts.Last();

            Assert.Equal("Trứng luộc", product1.ProductName);
            Assert.Equal(4000, product1.Price);

            Assert.Equal("Trứng rán", product2.ProductName);
            Assert.Equal(5000, product2.Price);
        }

    }
}
