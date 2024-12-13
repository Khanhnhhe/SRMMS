//using Xunit;
//using Moq;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using SRMMS.Controllers;
//using SRMMS.DTOs;
//using SRMMS.Models;
//using SRMMS.SMS;
//using SRMMS.Tests;
//namespace SRMMS.Tests
//{
//    public class TableControllerTests
//    {
//        private readonly Mock<SRMMSContext> _mockContext;
//        private readonly Mock<ITwilioService> _mockTwilioService; // Mock Twilio Service
//        private readonly TableController _controller;

//        public TableControllerTests()
//        {
//            // Initialize mock DbContext
//            _mockContext = new Mock<SRMMSContext>(new DbContextOptions<SRMMSContext>());

//            // Initialize mock Twilio Service
//            _mockTwilioService = new Mock<ITwilioService>();

//            // Initialize controller with all dependencies
//            _controller = new TableController(_mockContext.Object, _mockTwilioService.Object);
//        }

//        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
//        {
//            var mockSet = new Mock<DbSet<T>>();

//            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncEnumerator<T>(data.Provider));
//            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
//            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
//            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
//            mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
//                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

//            mockSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(data.ToList().Add);

//            return mockSet;
//        }


//        [Fact]
//        public async Task UpdateTable_ShouldReturnBadRequest_WhenTableNameIsEmpty()
//        {
//            // Arrange
//            var tableDTO = new TableDTO
//            {
//                Table_Name = "",
//                TableOfPeople = 4
//            };

//            // Act
//            var result = await _controller.UpdateTable(1, tableDTO);

//            // Assert
//            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//            Assert.Equal("Tên bàn không được để trống.", badRequestResult.Value);
//        }

//        [Fact]
//        public async Task UpdateTable_ShouldReturnBadRequest_WhenTableOfPeopleIsZeroOrNegative()
//        {
//            // Arrange
//            var tableData = new List<Table>
//        {
//            new Table { TableId = 1, TableName = "Bàn cũ", TableOfPeople = 4, StatusId = 1 }
//        }.AsQueryable();

//            var mockDbSet = CreateMockDbSet(tableData);
//            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

//            var tableDTO = new TableDTO
//            {
//                Table_Name = "Bàn 1",
//                TableOfPeople = 0
//            };

//            // Act
//            var result = await _controller.UpdateTable(1, tableDTO);

//            // Assert
//            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//            Assert.Equal("Chỗ người tại bàn phải là số nguyên dương và lớn hơn 0.", badRequestResult.Value);
//        }


//        [Fact]
//        public async Task UpdateTable_ShouldReturnOk_WhenUpdateIsSuccessful()
//        {
//            // Arrange
//            var table = new Table
//            {
//                TableId = 1,
//                TableName = "Bàn cũ",
//                TableOfPeople = 2,
//                StatusId = 1
//            };

//            var tableData = new List<Table> { table }.AsQueryable();
//            var mockDbSet = CreateMockDbSet(tableData);

//            _mockContext.Setup(c => c.Tables).Returns(mockDbSet.Object);

//            var tableDTO = new TableDTO
//            {
//                Table_Name = "Bàn 1",
//                TableOfPeople = 4,
//                StatusId = 2
//            };

//            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

//            // Act
//            var result = await _controller.UpdateTable(1, tableDTO);

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Cập nhật thành công ", okResult.Value);
//        }
//    }
//}