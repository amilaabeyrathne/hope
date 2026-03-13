using Moq;
using WorkdayCalendar.API.Services;

namespace WorkdayCalendar.Test
{
    public class WorkDayCalculatorServiceTest
    {
        [Fact]
        public void AddWorkdays_OneDayToTheCurrent_ToTheEndOfDay_ReturnsExpectedDate()
        {
            var calculator = CreateCalculator();

            var startDate = new DateTime(2004, 5, 24, 16, 0, 0);

            var result = calculator.CalculateWorkday(startDate, 1);

            var expected = new DateTime(2004, 5, 26, 8, 0, 0);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddWorkdays_OneDayToTheCurrent_InBetweenStartToEnd_ReturnsExpectedDate()
        {
            var calculator = CreateCalculator();

            var startDate = new DateTime(2004, 5, 24, 12, 0, 0);

            var result = calculator.CalculateWorkday(startDate, 1);

            var expected = new DateTime(2004, 5, 25, 12, 0, 0);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddWorkdays_OneDayNegativeToTheCurrent_ToTheEndOfDay_ReturnsExpectedDate()
        {
            var calculator = CreateCalculator();

            var startDate = new DateTime(2004, 5, 24, 17, 0, 0);

            var result = calculator.CalculateWorkday(startDate, -1);

            var expected = new DateTime(2004, 5, 24, 8, 0, 0);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddWorkdays_OneDayNegativeToTheCurrent_ToThMiddleOfDay_ReturnsExpectedDate()
        {
            var calculator = CreateCalculator();

            var startDate = new DateTime(2004, 5, 25, 12, 0, 0);

            var result = calculator.CalculateWorkday(startDate, -1);

            var expected = new DateTime(2004, 5, 24, 12, 0, 0);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddWorkdays_PartialToTheCurrent_ToThMiddleOfDay_ReturnsExpectedDate()
        {
            var calculator = CreateCalculator();

            var startDate = new DateTime(2004, 5, 24, 12, 0, 0);

            var result = calculator.CalculateWorkday(startDate, 0.25m);

            var expected = new DateTime(2004, 5, 24, 14, 0, 0);

            Assert.Equal(expected, result);
        }

        private WorkDayCalculatorService CreateCalculator()
        {
            var mockHolidayRegistry = new Mock<IHolidayRegistryService>();
            mockHolidayRegistry
                .Setup(h => h.IsWorkday(It.IsAny<DateTime>()))
                .Returns((DateTime d) =>
                    d.DayOfWeek != DayOfWeek.Saturday &&
                    d.DayOfWeek != DayOfWeek.Sunday &&
                    d.Date != new DateTime(2004, 5, 17) &&
                    d.Date != new DateTime(2004, 5, 27));

            var mockSettings = new Mock<IWorkdaySettingsService>();
            mockSettings.Setup(s => s.StartTime).Returns(TimeSpan.FromHours(8));
            mockSettings.Setup(s => s.StopTime).Returns(TimeSpan.FromHours(16));
            mockSettings.Setup(s => s.WorkSecondsPerDay).Returns(28800m);

            return new WorkDayCalculatorService(mockHolidayRegistry.Object, mockSettings.Object);
        }
    }
}
