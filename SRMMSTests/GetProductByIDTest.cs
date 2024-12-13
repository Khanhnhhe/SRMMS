using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.Controllers;
using SRMMS.DTOs;
using SRMMS.Models;
using System.Threading.Tasks;
using Xunit;

public class ProductControllerTests
{
   
    [Fact]
    public async Task GetProductById_ReturnsBadRequest_WhenProIdIsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_ProIdNull")
            .Options;

        using var context = new SRMMSContext(options);
        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductById(0); // Passing invalid ProId (0)

        // Assert
        var actionResult = Assert.IsType<ActionResult<ListProductDTO>>(result); // Check ActionResult type
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result); // Check Result type
      
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFoundMessage_WhenProIdDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_ProIdNotExist")
            .Options;

        using var context = new SRMMSContext(options);
        context.Products.Add(new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true });
        context.SaveChanges();

        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductById(999); // Passing non-existing ProId

        // Assert
        var actionResult = Assert.IsType<ActionResult<ListProductDTO>>(result); // Verify the ActionResult type
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Verify the result is OkObjectResult
        var response = okResult.Value as dynamic; // Cast the response to dynamic

        // Validate the response message
        Assert.NotNull(response);
        
    }

    [Fact]
    public async Task GetProductById_ReturnsNull_WhenProIdDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_ProIdNotExist")
            .Options;

        using var context = new SRMMSContext(options);
        context.Products.Add(new Product { ProId = 1, ProName = "Trứng rán", ProPrice = 5000, CatId = 1, ProStatus = true });
        context.SaveChanges();

        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductById(-1); // Passing non-existing ProId

        // Assert
        var actionResult = Assert.IsType<ActionResult<ListProductDTO>>(result); // Verify the ActionResult type
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Verify the result is OkObjectResult
        var response = okResult.Value as dynamic; // Cast the response to dynamic

        // Validate the response message
        Assert.NotNull(response);

    }

    [Fact]
    public async Task GetProductById_ReturnsProduct_WhenProIdExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_ProIdExists")
            .Options;

        using var context = new SRMMSContext(options);
        context.Products.Add(new Product
        {
            ProId = 1,
            ProName = "Trứng rán",
            ProPrice = 5000,
            CatId = 1,
            ProStatus = true,
            ProDiscription = "Delicious fried egg",
            ProImg = "",
            ProCalories = "200",
            Cat = new Category { CatId = 1, CatName = "Food" }
        });
        context.SaveChanges();

        var controller = new ProductController(context);

        // Act
        var result = await controller.GetProductById(1); // Passing existing ProId

        // Assert
        var actionResult = Assert.IsType<ActionResult<ListProductDTO>>(result); // Verify the ActionResult type
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Verify the result is OkObjectResult
        var product = Assert.IsType<ListProductDTO>(okResult.Value); // Verify the response is ListProductDTO

        // Validate the product details
        Assert.Equal(1, product.ProductId);
        Assert.Equal("Trứng rán", product.ProductName);
        Assert.Equal("Delicious fried egg", product.Description);
        Assert.Equal(5000, product.Price);
        Assert.Equal("Food", product.Category);
        Assert.Equal("", product.Image);
        Assert.Equal("200", product.Calories);
        Assert.True(product.Status);
    }
}
