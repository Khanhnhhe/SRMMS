//using Xunit;
//using Moq;

//using SRMMS.Models;
//using S.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using NuGet.Protocol.Plugins;
//using SRMMS.Controllers;
//using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
//using System.Security.Authentication;

//public class LoginControllerTests
//{
//    private readonly Mock<IUserService> _userServiceMock;
//    private readonly LoginController _controller;

//    public LoginControllerTests()
//    {
//        _userServiceMock = new Mock<IUserService>();
//        _controller = new LoginController(_userServiceMock.Object);
//    }

//    [Fact]
//    public async Task Login_ReturnsNotFound_WhenUserDoesNotExist()
//    {
//        // Arrange
//        var phone = "0999999999";
//        var password = "11T111111111";
//        _userServiceMock.Setup(service => service.AuthenticateUserAsync(phone, password))
//                        .ReturnsAsync((User)null);

//        // Act
//        var result = await _controller.Login(new LoginRequest { Phone = phone, Password = password });

//        // Assert
//        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
//        Assert.Equal("Không tìm thấy người dùng", notFoundResult.Value);
//    }

//    [Fact]
//    public async Task Login_ReturnsBadRequest_WhenAccountIsInactive()
//    {
//        // Arrange
//        var phone = "0899297998";
//        var password = "11T111111111";
//        var inactiveUser = new User { Phone = phone, IsActive = false };
//        _userServiceMock.Setup(service => service.AuthenticateUserAsync(phone, password))
//                        .ReturnsAsync(inactiveUser);

//        // Act
//        var result = await _controller.Login(new LoginRequest { Phone = phone, Password = password });

//        // Assert
//        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//        Assert.Equal("Tài khoản không hoạt động", badRequestResult.Value);
//    }

//    [Fact]
//    public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
//    {
//        // Arrange
//        var phone = "0856287188";
//        var password = "322222223f";
//        _userServiceMock.Setup(service => service.AuthenticateUserAsync(phone, password))
//                        .ThrowsAsync(new InvalidCredentialException("Mật khẩu không hợp lệ"));

//        // Act
//        var result = await _controller.Login(new LoginRequest { Phone = phone, Password = password });

//        // Assert
//        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
//        Assert.Equal("Mật khẩu không hợp lệ", unauthorizedResult.Value);
//    }

//    [Fact]
//    public async Task Login_ReturnsToken_WhenLoginIsSuccessful()
//    {
//        // Arrange
//        var phone = "0856287188";
//        var password = "K123123123";
//        var user = new User { Phone = phone, IsActive = true };
//        _userServiceMock.Setup(service => service.AuthenticateUserAsync(phone, password))
//                        .ReturnsAsync(user);
//        _userServiceMock.Setup(service => service.GenerateToken(user))
//                        .Returns("mock_token");

//        // Act
//        var result = await _controller.Login(new LoginRequest { Phone = phone, Password = password });

//        // Assert
//        var okResult = Assert.IsType<OkObjectResult>(result);
//        Assert.Equal("mock_token", okResult.Value);
//    }
//}
