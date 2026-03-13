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

        [Fact]
        public void AddWorkdays_Forward_TwoConsecutiveRecurringHolidaysAfterWeekend_SkipsToFirstWorkday()
        {
            // Friday 21 May 12:00 + 1 workday: next workday is Monday, but 24–25 May are recurring holidays
            // (Mon & Tue), so we skip to Wednesday 26 May 12:00
            var calculator = CreateCalculatorWithConsecutiveRecurringHolidays();

            var startDate = new DateTime(2004, 5, 21, 12, 0, 0); // Friday

            var result = calculator.CalculateWorkday(startDate, 1);

            var expected = new DateTime(2004, 5, 26, 12, 0, 0); // Wednesday

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddWorkdays_Backward_TwoConsecutiveRecurringHolidaysAfterWeekend_SkipsToPreviousWorkday()
        {
            // Wednesday 26 May 12:00 - 1 workday: previous workday is Tuesday, but 25–24 May are recurring holidays
            // (Tue & Mon), so we skip back to Friday 21 May 12:00
            var calculator = CreateCalculatorWithConsecutiveRecurringHolidays();

            var startDate = new DateTime(2004, 5, 26, 12, 0, 0); // Wednesday

            var result = calculator.CalculateWorkday(startDate, -1);

            var expected = new DateTime(2004, 5, 21, 12, 0, 0); // Friday

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddWorkdays_LeapYear_ForwardLandsOnFebruary29()
        {
            // 2024 is a leap year. Wed 28 Feb 12:00 + 1 workday = Thu 29 Feb 12:00
            var calculator = CreateCalculator();

            var startDate = new DateTime(2024, 2, 28, 12, 0, 0); // Wednesday

            var result = calculator.CalculateWorkday(startDate, 1);

            var expected = new DateTime(2024, 2, 29, 12, 0, 0); // Leap day

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddWorkdays_LeapYear_RecurringHolidayOnFebruary29_SkipsToMarch1()
        {
            // 2024 is a leap year. Feb 29 is recurring holiday. Wed 28 Feb 12:00 + 1 workday = Fri 1 Mar 12:00
            var calculator = CreateCalculatorWithFebruary29Holiday();

            var startDate = new DateTime(2024, 2, 28, 12, 0, 0); // Wednesday

            var result = calculator.CalculateWorkday(startDate, 1);

            var expected = new DateTime(2024, 3, 1, 12, 0, 0); // Friday

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

        private static WorkDayCalculatorService CreateCalculatorWithConsecutiveRecurringHolidays()
        {
            var mockHolidayRegistry = new Mock<IHolidayRegistryService>();
            mockHolidayRegistry
                .Setup(h => h.IsWorkday(It.IsAny<DateTime>()))
                .Returns((DateTime d) =>
                    d.DayOfWeek != DayOfWeek.Saturday &&
                    d.DayOfWeek != DayOfWeek.Sunday &&
                    d.Date != new DateTime(2004, 5, 24) &&
                    d.Date != new DateTime(2004, 5, 25));

            var mockSettings = new Mock<IWorkdaySettingsService>();
            mockSettings.Setup(s => s.StartTime).Returns(TimeSpan.FromHours(8));
            mockSettings.Setup(s => s.StopTime).Returns(TimeSpan.FromHours(16));
            mockSettings.Setup(s => s.WorkSecondsPerDay).Returns(28800m);

            return new WorkDayCalculatorService(mockHolidayRegistry.Object, mockSettings.Object);
        }

        private static WorkDayCalculatorService CreateCalculatorWithFebruary29Holiday()
        {
            var mockHolidayRegistry = new Mock<IHolidayRegistryService>();
            mockHolidayRegistry
                .Setup(h => h.IsWorkday(It.IsAny<DateTime>()))
                .Returns((DateTime d) =>
                    d.DayOfWeek != DayOfWeek.Saturday &&
                    d.DayOfWeek != DayOfWeek.Sunday &&
                    !(d.Month == 2 && d.Day == 29));

            var mockSettings = new Mock<IWorkdaySettingsService>();
            mockSettings.Setup(s => s.StartTime).Returns(TimeSpan.FromHours(8));
            mockSettings.Setup(s => s.StopTime).Returns(TimeSpan.FromHours(16));
            mockSettings.Setup(s => s.WorkSecondsPerDay).Returns(28800m);

            return new WorkDayCalculatorService(mockHolidayRegistry.Object, mockSettings.Object);
        }
    }
}
