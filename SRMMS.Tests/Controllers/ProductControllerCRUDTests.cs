using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SRMMS.Controllers;
using SRMMS.Models;
using SRMMS.DTOs;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

public class ProductControlleCRUDTests
{
    private readonly Mock<Cloudinary> _mockCloudinary;
    public ProductControlleCRUDTests()
    {
       

      
    }
    public SRMMSContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new SRMMSContext(options);

        // Xóa tất cả dữ liệu hiện có để tránh dữ liệu trùng lặp
        context.Categories.RemoveRange(context.Categories);
        context.Products.RemoveRange(context.Products);
        context.SaveChanges();

        // Thêm dữ liệu mẫu vào cơ sở dữ liệu
        context.Categories.Add(new Category { CatId = 1, CatName = "Hải sản" });
        context.Categories.Add(new Category { CatId = 2, CatName = "No Products" });

        context.Products.Add(new Product
        {
            ProId = 1,
            ProName = "Test Product",
            ProDiscription = "Description",
            ProPrice = 100,
            ProImg = "image.jpg",
            ProCalories = "200",
            ProStatus = true,
            CatId = 1
        });
        context.Products.Add(new Product
        {
            ProId = 2,
            ProName = "Cua Hoàng Đế",
            ProDiscription = "Description",
            ProPrice = 200,
            ProImg = "image.jpg",
            ProCalories = "300",
            ProStatus = true,
            CatId = 1
        });
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetAllProducts_WithCategoryFilter_ReturnsFilteredResults()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new SRMMSContext(options))
        {
            context.Categories.Add(new Category { CatId = 1, CatName = "Food" });
            context.Products.AddRange(
                new Product { ProId = 1, ProName = "Pizza", ProPrice = 10000, CatId = 1 },
                new Product { ProId = 2, ProName = "Burger", ProPrice = 8000, CatId = 1 },
                new Product { ProId = 3, ProName = "Sushi", ProPrice = 15000, CatId = 2 }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new SRMMSContext(options))
        {
            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(categoryId: 1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = actionResult.Value;

            Assert.NotNull(response);
            Assert.Equal(2, (int)response.TotalProducts); 
            Assert.Equal(1, (int)response.TotalPages);
            Assert.Equal(1, (int)response.PageNumber);
            Assert.NotEmpty(response.Products);
            Assert.Equal(2, response.Products.Count);
        }
    }

    [Fact]
    public async Task GetAllProducts_NoMatchingProducts_ReturnsEmptyList()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new SRMMSContext(options))
        {
            context.Products.Add(new Product { ProId = 1, ProName = "Pizza", ProPrice = 10000, CatId = 1 });
            await context.SaveChangesAsync();
        }

        using (var context = new SRMMSContext(options))
        {
            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(categoryId: 999);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var response = actionResult.Value as dynamic;

            Assert.NotNull(response);
            Assert.Equal(0, response.TotalProducts);
            Assert.Empty(response.Products);
        }
    }
    [Fact]
    public async Task GetAllProducts_WithInvalidMinPrice_ReturnsBadRequest()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new SRMMSContext(options))
        {
            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(minPrice: "invalid");

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid minPrice. Please enter a numeric value.", actionResult.Value);
        }
    }
    [Fact]
    public async Task GetAllProducts_MatchingProducts_ReturnsProduct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new SRMMSContext(options))
        {
            context.Products.AddRange(
             new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1 },
             new Product { ProId = 2, ProName = "Bánh mì", ProPrice = 10000, CatId = 1 },
             new Product { ProId = 3, ProName = "Trứng rán với hành", ProPrice = 7000, CatId = 2 }
         );
            await context.SaveChangesAsync();
        }

        using (var context = new SRMMSContext(options))
        {
            var controller = new ProductController(context);

            // Act
            var result = await controller.GetAllProducts(name: "Trứng rán");

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaginatedProductsResult>(actionResult.Value);

            Assert.Equal(2, response.TotalProducts);
            Assert.NotNull(response.Products);


        }
    }

    [Fact]
    public async Task AddProduct_ProductNameExists_ReturnsBadRequest()
    {
        // Arrange
        var newProduct = new addProductDTO
        {
            ProductName = "Test Product", // Tên sản phẩm đã tồn tại
            Description = "New Description",
            Price = 12000,
            Category = 1,
            Image = new FormFile(new MemoryStream(), 0, 0, "image", "newImage.jpg"),
            Calories = "Moderate",
            Status = true
        };

        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.AddProduct(newProduct);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("A product with this name already exists.", badRequest.Value);
    }
    [Fact]
    public async Task AddProduct_ValidProduct_ReturnsCreatedResult()
    {
        // Arrange
        var newProduct = new addProductDTO
        {
            ProductName = "Valid Product",
            Description = "Valid Description",
            Price = 15000,
            Category = 1,
            Image = new FormFile(new MemoryStream(), 0, 0, "image", "validImage.jpg"),
            Calories = "Low",
            Status = true
        };

        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.AddProduct(newProduct);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetProductById", createdResult.ActionName);

        var createdProduct = Assert.IsType<Product>(createdResult.Value);
        Assert.Equal("Valid Product", createdProduct.ProName);
        Assert.Equal(15000, createdProduct.ProPrice);
    }


}
