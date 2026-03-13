using Moq;
using WorkdayCalendar.API.Services;

namespace WorkdayCalendar.Test
{
    public class ExceptionTests
    {
        [Fact]
        public void CalculateWorkday_WhenWorkdayHoursNotConfigured_ThrowsInvalidOperationException()
        {
            var mockHolidayRegistry = new Mock<IHolidayRegistryService>();
            mockHolidayRegistry.Setup(h => h.IsWorkday(It.IsAny<DateTime>())).Returns(true);

            var mockSettings = new Mock<IWorkdaySettingsService>();
            mockSettings.Setup(s => s.StartTime).Returns(TimeSpan.Zero);
            mockSettings.Setup(s => s.StopTime).Returns(TimeSpan.Zero);
            mockSettings.Setup(s => s.WorkSecondsPerDay).Returns(0m);

            var calculator = new WorkDayCalculatorService(mockHolidayRegistry.Object, mockSettings.Object);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                calculator.CalculateWorkday(new DateTime(2004, 5, 24, 12, 0, 0), 1));

            Assert.Equal("Workday hours have not been configured.", ex.Message);
        }

        [Fact]
        public void CalculateWorkday_WhenTooManyConsecutiveNonWorkingDaysInGetValidWorkingWindow_ThrowsInvalidOperationException()
        {
            var mockHolidayRegistry = new Mock<IHolidayRegistryService>();
            mockHolidayRegistry.Setup(h => h.IsWorkday(It.IsAny<DateTime>())).Returns(false);

            var mockSettings = new Mock<IWorkdaySettingsService>();
            mockSettings.Setup(s => s.StartTime).Returns(TimeSpan.FromHours(8));
            mockSettings.Setup(s => s.StopTime).Returns(TimeSpan.FromHours(16));
            mockSettings.Setup(s => s.WorkSecondsPerDay).Returns(28800m);

            var calculator = new WorkDayCalculatorService(mockHolidayRegistry.Object, mockSettings.Object);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                calculator.CalculateWorkday(new DateTime(2004, 5, 24, 12, 0, 0), 1));

            Assert.Equal("Too many non-working days in a row. Please check your holiday configuration.", ex.Message);
        }

    }
}
