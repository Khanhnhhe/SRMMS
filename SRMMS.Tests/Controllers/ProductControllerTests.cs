using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SRMMS.Controllers;
using SRMMS.DTOs;
using SRMMS.Models;
using System.Threading.Tasks;
using Xunit;

public class ProductControllerTests
{

    private SRMMSContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        var context = new SRMMSContext(options);

        // Clear existing data to avoid duplicates
        context.Categories.RemoveRange(context.Categories);
        context.Products.RemoveRange(context.Products);
        context.SaveChanges();

        // Seed data
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
    public async Task UTCID01_ProIdIsBlank_ReturnsBadRequest()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductById(0);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("The parameter 'proId' is required.", actionResult.Value);
    }

    [Fact]
    public async Task UTCID02_ProIdIsVeryLargeNumber_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductById(999999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UTCID03_ProIdExists_ReturnsProductInfo()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductById(1);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<ListProductDTO>(actionResult.Value);

        Assert.Equal(1, product.ProductId);
        Assert.Equal("Test Product", product.ProductName);
    }

    [Fact]
    public async Task UTCID04_ProIdIsNegative_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductById(-1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
    [Fact]
    public async Task UTCID05_NonExistingCategory_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.FilterByCategoryName("Non-Existing Category");

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Category not found.", actionResult.Value);
    }

    [Fact]
    public async Task UTCID06_CategoryWithNoProducts_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.FilterByCategoryName("No Products");

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No products found in this category.", actionResult.Value);
    }

    [Fact]
    public async Task UTCID07_CategoryWithProducts_ReturnsProductList()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.FilterByCategoryName("Hải sản", 1, 10);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ProductFilterResultDTO>(actionResult.Value);

        Assert.Equal(2, response.TotalProducts);
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(10, response.PageSize);
        Assert.NotEmpty(response.Products);
    }


    [Fact]
    public async Task UTCID08_PageOutOfRange_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.FilterByCategoryName("Hải sản", 11111, 10);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No products found in this category.", actionResult.Value);
    }
    [Fact]
    public async Task UTCID09_ProductNameIsEmpty_ReturnsAllProducts()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.SearchByProductName("", 1, 10);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsType<List<ListProductDTO>>(actionResult.Value);

        Assert.Equal(2, products.Count);
    }
    [Fact]
    public async Task UTCID10_ProductNameIsNonExisting_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.SearchByProductName("Non Existing Product", 1, 10);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No products found.", actionResult.Value);
    }
    [Fact]
    public async Task UTCID11_ProductNameMatches_ReturnsProductList()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.SearchByProductName("Test Product", 1, 10);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsType<List<ListProductDTO>>(actionResult.Value);

        Assert.Single(products);
        Assert.Equal("Test Product", products[0].ProductName);
    }
    [Fact]
    public async Task UTCID12_PageSizeIsZero_ReturnsBadRequest()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.SearchByProductName("", 1, 0);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Page size must be greater than zero.", actionResult.Value);
    }
    [Fact]
    public async Task UTCID013_ProIdIsZero_ReturnsBadRequest()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductDetail(0);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid product ID. ID must be greater than 0.", actionResult.Value);
    }
    [Fact]
    public async Task UTCID14_ProIdIsVeryLarge_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductDetail(999999);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product not found", actionResult.Value);
    }

    [Fact]
    public async Task UTCID15_ProIdExists_ReturnsProductDetailWithRelatedProducts()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductDetail(1);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        dynamic response = actionResult.Value;

        // Validate ProductDetail
        Assert.NotNull(response.ProductDetail);
        Assert.Equal(1, response.ProductDetail.ProductId);
        Assert.Equal("Test Product", response.ProductDetail.ProductName);

        // Validate RelatedProducts
        Assert.NotEmpty(response.RelatedProducts);
        Assert.IsType<List<ListProductDTO>>(response.RelatedProducts);
    }
    [Fact]
    public async Task UTCID16_ProIdIsNegative_ReturnsBadRequest()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductDetail(-1);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid product ID. ID must be greater than 0.", actionResult.Value);
    }

    [Fact]
    public async Task UTCID17_DeleteProduct_ProIdExists_DeletesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);
        var productId = 1;

        // Act
        var result = await controller.DeleteProduct(productId);

        // Assert
        var actionResult = Assert.IsType<NoContentResult>(result);

        // Validate the product is deleted
        var deletedProduct = await context.Products.FindAsync(productId);
        Assert.Null(deletedProduct);  // Ensure product is deleted
    }
    [Fact]
    public async Task UTCID18_DeleteProduct_ProIdNotFound_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);
        var productId = -1; // Non-existing product

        // Act
        var result = await controller.DeleteProduct(productId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product not found.", actionResult.Value);
    }
    [Fact]
    public async Task DeleteProduct_ProIdTooLarge_ReturnsBadRequest()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ProductController(context);

        // Simulate a very large product ID that exceeds database limit
        var largeProductId = int.MaxValue;  // Check with int.MaxValue or any other very large number

        // Act
        var result = await controller.DeleteProduct(largeProductId); // Pass the large id

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The passed proId is a very large number (exceeding the database limit).", actionResult.Value);


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

