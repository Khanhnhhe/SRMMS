using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SRMMS.Controllers;
using SRMMS.Models;
using SRMMS.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;

namespace SRMMS.Tests.Controller
{
    public class ProductCategoriesControllerTests
    {
        [Fact]
        public async Task GetCategories_ReturnsCategories_WhenCategoriesExist()
        {
            // Arrange
            var mockContext = new Mock<SRMMSContext>();

            var categories = new List<Category>
    {
        new Category { CatId = 1, CatName = "Category 1", Description = "Description 1" },
        new Category { CatId = 2, CatName = "Category 2", Description = "Description 2" }
    }.AsQueryable();

            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var controller = new CategoryController(mockContext.Object);

            // Act
            var result = await controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ProductCategoriesDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async Task GetCategories_ReturnsNotFound_WhenNoCategoriesExist()
        {
            // Arrange
            var mockContext = new Mock<SRMMSContext>();

            var categories = new List<Category>().AsQueryable(); // Empty list

            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var controller = new CategoryController(mockContext.Object);

            // Act
            var result = await controller.GetCategories();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No categories found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCategories_ReturnsPaginatedCategories_WhenPagedParametersAreGiven()
        {
            // Arrange
            var mockContext = new Mock<SRMMSContext>();

            var categories = new List<Category>
    {
        new Category { CatId = 1, CatName = "Category 1", Description = "Description 1" },
        new Category { CatId = 2, CatName = "Category 2", Description = "Description 2" },
        new Category { CatId = 3, CatName = "Category 3", Description = "Description 3" },
    }.AsQueryable();

            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var controller = new CategoryController(mockContext.Object);

            int pageNumber = 1;
            int pageSize = 2;

            // Act
            var result = await controller.GetCategories(pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ProductCategoriesDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count); // Verify page size
            Assert.Equal("Category 1", returnValue[0].CatName); 
        }

        [Fact]
        public async Task GetCategories_ReturnsAllCategories_WhenNoPaginationOrFilterApplied()
        {
            // Arrange
            var mockContext = new Mock<SRMMSContext>();

            var categories = new List<Category>
    {
        new Category { CatId = 1, CatName = "Category 1", Description = "Description 1" },
        new Category { CatId = 2, CatName = "Category 2", Description = "Description 2" },
        new Category { CatId = 3, CatName = "Category 3", Description = "Description 3" },
    }.AsQueryable();

            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var controller = new CategoryController(mockContext.Object);

            // Act
            var result = await controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ProductCategoriesDTO>>(okResult.Value);
            Assert.Equal(3, returnValue.Count); 
        }
        [Fact]
        public async Task AddCategory_ReturnsBadRequest_WhenCategoryAlreadyExists()
        {
            // Arrange
            var mockContext = new Mock<SRMMSContext>();
            var mockDbSet = new Mock<DbSet<Category>>();

            var existingCategories = new List<Category>
    {
        new Category { CatId = 1, CatName = "Existing Category", Description = "Already exists" }
    }.AsQueryable();

            mockContext.Setup(x => x.Categories).ReturnsDbSet(existingCategories);

            var controller = new CategoryController(mockContext.Object);
            var categoryDto = new ProductCategoriesDTO
            {
                CatName = "Existing Category",  // Giả lập category đã tồn tại
                Description = "Description of existing category"
            };

            // Act
            var result = await controller.AddCategory(categoryDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("A category with this name already exists.", badRequestResult.Value);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task AddCategory_ReturnsCreatedAtAction_WhenCategoryIsAddedSuccessfully()
        {
            var mockContext = new Mock<SRMMSContext>();

            var categories = new List<Category>
    {
        new Category { CatId = 1, CatName = "Existing Category" }
    }.AsQueryable();

            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var newCategoryDto = new ProductCategoriesDTO
            {
                CatName = "New Category",
                Description = "New Description"
            };

            var controller = new CategoryController(mockContext.Object);

            // Act
            var result = await controller.AddCategory(newCategoryDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var categoryResult = Assert.IsType<ProductCategoriesDTO>(createdAtActionResult.Value);
            Assert.Equal("New Category", categoryResult.CatName);

            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddCategory_ReturnsBadRequest_WhenCategoryNameAlreadyExists()
        {
            // Arrange
            var mockContext = new Mock<SRMMSContext>();

            var categories = new List<Category>
    {
        new Category { CatId = 1, CatName = "Existing Category" }
    }.AsQueryable();

            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var duplicateCategoryDto = new ProductCategoriesDTO
            {
                CatName = "Existing Category", // Duplicate name
                Description = "New Description"
            };

            var controller = new CategoryController(mockContext.Object);

            // Act
            var result = await controller.AddCategory(duplicateCategoryDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("A category with this name already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddCategory_ReturnsBadRequest_WhenDbUpdateFails()
        {
            // Arrange
            var mockContext = new Mock<SRMMSContext>();
            mockContext.Setup(x => x.Categories).ReturnsDbSet(new List<Category>());
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new DbUpdateException()); 

            var newCategoryDto = new ProductCategoriesDTO
            {
                CatName = "New Category",
                Description = "Valid Description"
            };

            var controller = new CategoryController(mockContext.Object);

            // Act
            var result = await controller.AddCategory(newCategoryDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Could not save the category.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsCategory_WhenCategoryExists()
        {
            // Arrange
            var mockContext = new Mock<SRMMSContext>();
            var categoryId = 1;

            var categories = new List<Category>
    {
        new Category { CatId = categoryId, CatName = "Category 1", Description = "Description 1" }
    }.AsQueryable();

            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var controller = new CategoryController(mockContext.Object);

            // Act
            var result = await controller.GetCategoryById(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ProductCategoriesDTO>(okResult.Value);
            Assert.Equal(categoryId, returnValue.CatId);
            Assert.Equal("Category 1", returnValue.CatName);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            var mockContext = new Mock<SRMMSContext>();
            var categoryId = 1;

            var categories = new List<Category>().AsQueryable(); 
            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var controller = new CategoryController(mockContext.Object);

            var result = await controller.GetCategoryById(categoryId);

            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsCorrectDataModel_WhenCategoryExists()
        {
            var mockContext = new Mock<SRMMSContext>();
            var categoryId = 2;

            var categories = new List<Category>
    {
        new Category { CatId = categoryId, CatName = "Category 2", Description = "Second Category Description" }
    }.AsQueryable();

            mockContext.Setup(x => x.Categories).ReturnsDbSet(categories);

            var controller = new CategoryController(mockContext.Object);

            var result = await controller.GetCategoryById(categoryId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ProductCategoriesDTO>(okResult.Value);
            Assert.Equal(categoryId, returnValue.CatId);
            Assert.Equal("Category 2", returnValue.CatName);
            Assert.Equal("Second Category Description", returnValue.Description);
        }

        [Fact]
        public async Task DeleteCategoryById_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            var mockContext = new Mock<SRMMSContext>();

            mockContext.Setup(x => x.Categories.FindAsync(It.IsAny<int>()))
                       .ReturnsAsync((Category)null);

            var controller = new CategoryController(mockContext.Object);

            var result = await controller.DeleteCategoryById(1);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Category not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteCategoryById_ReturnsNoContent_WhenCategoryAndProductsDeletedSuccessfully()
        {
            var mockContext = new Mock<SRMMSContext>();
            var category = new Category { CatId = 1, CatName = "Category 1" };
            var products = new List<Product>
        {
            new Product { ProId = 1, CatId = 1 },
            new Product { ProId = 2, CatId = 1 }
        }.AsQueryable();

            mockContext.Setup(x => x.Categories.FindAsync(It.IsAny<int>()))
                       .ReturnsAsync(category);
            mockContext.Setup(x => x.Products)
                       .ReturnsDbSet(products);

            var controller = new CategoryController(mockContext.Object);

            var result = await controller.DeleteCategoryById(1);

            Assert.IsType<NoContentResult>(result);
            mockContext.Verify(m => m.Categories.Remove(category), Times.Once);
            mockContext.Verify(m => m.Products.RemoveRange(It.IsAny<IEnumerable<Product>>()), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryById_ReturnsBadRequest_WhenDbUpdateExceptionOccurs()
        {

            var mockContext = new Mock<SRMMSContext>();
            var category = new Category { CatId = 1, CatName = "Category 1" };

            mockContext.Setup(x => x.Categories.FindAsync(It.IsAny<int>()))
                       .ReturnsAsync(category);

            var products = new List<Product>().AsQueryable();
            mockContext.Setup(x => x.Products).ReturnsDbSet(products);

            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new DbUpdateException());

            var controller = new CategoryController(mockContext.Object);


            var result = await controller.DeleteCategoryById(1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not delete the category and its products.", badRequestResult.Value);
        }

        [Fact]
        public Task SearchCategoryByName_ReturnsCategories_WhenMatchingCategoriesExist()
        {
            var mockContext = new Mock<SRMMSContext>();


            var categories = new List<Category>
{
    new Category { CatId = 1, CatName = "Category 1", Description = "Description 1" },
    new Category { CatId = 2, CatName = "Category 2", Description = "Description 2" }
}.AsQueryable();

            var mockSet = new Mock<DbSet<Category>>();
            mockSet.As<IAsyncEnumerable<Category>>()
                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                   .Returns(new TestAsyncEnumerator<Category>(categories.GetEnumerator()));
            mockSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Category>(categories.Provider));
            mockSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
            mockSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            mockSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            mockContext.Setup(x => x.Categories).Returns(mockSet.Object);
            return Task.CompletedTask;
        }

    
        [Fact]
        public async Task UpdateCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            var mockContext = new Mock<SRMMSContext>();
            var categoryId = 1;

            mockContext.Setup(x => x.Categories.FindAsync(categoryId)).ReturnsAsync((Category)null);

            var controller = new CategoryController(mockContext.Object);
            var categoryDto = new ProductCategoriesDTO { CatName = "Nonexistent Category", Description = "Description" };

            var result = await controller.UpdateCategory(categoryId, categoryDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Category not found.", notFoundResult.Value);
        }

      

    }




}
