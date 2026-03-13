using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkdayCalendar.API.Controllers;
using WorkdayCalendar.API.Models;
using WorkdayCalendar.API.Services;

namespace WorkdayCalendar.Test
{
    public class ControllerTests
    {
        private readonly Mock<IHolidayRegistryService> _mockHolidayRegistry;
        private readonly Mock<IWorkdaySettingsService> _mockWorkdaySettings;
        private readonly Mock<IWorkDayCalculatorService> _mockCalculator;
        private readonly WorkdayController _controller;

        public ControllerTests()
        {
            _mockHolidayRegistry = new Mock<IHolidayRegistryService>();
            _mockWorkdaySettings = new Mock<IWorkdaySettingsService>();
            _mockCalculator = new Mock<IWorkDayCalculatorService>();
            _controller = new WorkdayController(
                _mockHolidayRegistry.Object,
                _mockWorkdaySettings.Object,
                _mockCalculator.Object);
        }

        [Fact]
        public void AddHoliday_ValidRequest_ReturnsOk()
        {
            var request = new HolidaysRequest { Holidays = [new DateTime(2004, 5, 27)] };

            var result = _controller.AddHoliday(request);

            Assert.IsType<OkResult>(result);
            _mockHolidayRegistry.Verify(
                h => h.AddHolidays(It.Is<IEnumerable<DateOnly>>(d => d.Single() == DateOnly.FromDateTime(new DateTime(2004, 5, 27)))),
                Times.Once);
        }

        [Fact]
        public void AddRecurringHoliday_ValidRequest_ReturnsOk()
        {
            var request = new RecurringHolidaysRequest
            {
                RecurringHolidays = [new RecurringHoliday { Month = 5, Day = 17 }]
            };

            var result = _controller.AddRecurringHoliday(request);

            Assert.IsType<OkResult>(result);
            _mockHolidayRegistry.Verify(
                h => h.AddRecurringHolidays(It.Is<IEnumerable<(int Month, int Day)>>(x =>
                    x.Count() == 1 && x.First().Month == 5 && x.First().Day == 17)),
                Times.Once);
        }

        [Fact]
        public void SetWorkdayHours_ValidRequest_ReturnsOk()
        {
            var request = new WorkdayHoursRequest
            {
                Start = TimeSpan.FromHours(8),
                End = TimeSpan.FromHours(16)
            };

            var result = _controller.SetWorkdayHours(request);

            Assert.IsType<OkResult>(result);
            _mockWorkdaySettings.Verify(
                s => s.SetWorkdayHours(TimeSpan.FromHours(8), TimeSpan.FromHours(16)),
                Times.Once);
        }

        [Fact]
        public void Calculate_ValidRequest_ReturnsOkWithCalculatedResult()
        {
            var start = new DateTime(2004, 5, 24, 12, 0, 0);
            var expected = new DateTime(2004, 5, 25, 12, 0, 0);
            _mockCalculator.Setup(c => c.CalculateWorkday(start, 1)).Returns(expected);

            var result = _controller.Calculate(start, 1);

            var okResult = Assert.IsType<ActionResult<DateTime>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var calculateResult = Assert.IsType<CalculateResult>(okObjectResult.Value);
            Assert.Equal(expected, calculateResult.Result);
            _mockCalculator.Verify(c => c.CalculateWorkday(start, 1), Times.Once);
        }
    }
}
