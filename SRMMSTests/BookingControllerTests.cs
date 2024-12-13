using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using SRMMS.Controllers;
using SRMMS.DTOs;
using SRMMS.Hubs;
using SRMMS.Models;
using SRMMS.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMMSTests
{
   public class BookingControllerTests
{
    [Theory]
    [InlineData("0856287188", "2024-11-30 20:00", 4, "John Doe", true)] // Valid case
    [InlineData("0899297998000", "2024-11-30 20:00", 4, "John Doe", false)] // Invalid phone number
    [InlineData("0856287188", "2023-11-30 20:00", 4, "John Doe", false)] // Past date
    [InlineData("0856287188", "2024-11-30 6:00", 4, "John Doe", false)] // Past time
    [InlineData("0856287188", "2024-11-30 20:00", 0, "John Doe", false)] // Invalid number of people
    [InlineData("0856287188", "2024-11-30 20:00", -9, "John Doe", false)] // Invalid number of people
    [InlineData("0856287188", "2024-11-30 20:00", 4, "", false)] // Empty name
    public async Task CreateBooking_ReturnsAppropriateResponse_WhenInputIsInvalidOrValid(
        string phone, string dateTime, int numberOfPeople, string name, bool isValid)
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SRMMSContext>()
            .UseInMemoryDatabase("ValidBookingDb")
            .Options;

        using var context = new SRMMSContext(options);
        var hubContextMock = new Mock<IHubContext<BookingHub>>();
        var twilioServiceMock = new Mock<ITwilioService>();

        var controller = new BookingController(context, hubContextMock.Object, twilioServiceMock.Object);

        var bookingDto = new CreateBookingDTO
        {
            PhoneBooking = phone,
            DayBooking = DateTime.Parse(dateTime),
            HourBooking = DateTime.Parse(dateTime).ToString("HH:mm"),
            NumberOfPeople = numberOfPeople,
            NameBooking = name
        };

        // Act
        var result = await controller.CreateBooking(bookingDto);

        // Assert
        if (isValid)
        {
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var booking = Assert.IsType<Booking>(createdResult.Value);
            Assert.Equal(name, booking.NameBooking);
            hubContextMock.Verify(h => h.Clients.All.SendAsync("ReceiveBookingUpdate", It.IsAny<object>(), default), Times.Once);
        }
        else
        {
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = badRequestResult.Value.ToString();
            
            if (string.IsNullOrEmpty(name))
                Assert.Contains("Vui lòng cung cấp tên của bạn.", errorMessage);
            else if (!IsValidPhoneNumber(phone))
                Assert.Contains("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại hợp lệ.", errorMessage);
            else if (numberOfPeople <= 0)
                Assert.Contains("Số người đặt bàn phải là số nguyên dương và lớn hơn 0.", errorMessage);
            else
                Assert.Contains("Ngày và giờ đặt bàn không hợp lệ.", errorMessage);
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10,11}$");
    }
}

}

