using WorkdayCalendar.API.Models;
using WorkdayCalendar.API.Services;

namespace WorkdayCalendar.Test
{
    public class WorkDayExampleFromDocumentTest
    {
        private readonly WorkDayCalculatorService _calculator;

        public WorkDayExampleFromDocumentTest()
        {
            var holidayRegistry = new HolidayRegistryService();
            holidayRegistry.AddRecurringHoliday(new RecurringHolidaysRequest
            {
                RecurringHolidays = [new RecurringHoliday { Month = 5, Day = 17 }]
            });
            holidayRegistry.AddHoliday(new HolidaysRequest
            {
                Holidays = [new DateTime(2004, 5, 27)]
            });

            var workdaySettings = new WorkdaySettingsService();
            workdaySettings.SetWorkdayHours(TimeSpan.FromHours(8), TimeSpan.FromHours(16));

            _calculator = new WorkDayCalculatorService(holidayRegistry, workdaySettings);
        }

        [Theory]
        [InlineData(18, 5,  -5.5,       2004, 5, 14, 12,  0)]
        [InlineData(19, 3,  44.723656,  2004, 7, 27, 13, 47)]
        [InlineData(18, 3,  -6.7470217, 2004, 5, 13, 10,  2)]
        [InlineData( 8, 3,  12.782709,  2004, 6, 10, 14, 18)]
        [InlineData( 7, 3,   8.276628,  2004, 6,  4, 10, 12)]
        public void AddWorkdays_DocumentExamples_ReturnsExpectedDate(
            int startHour, int startMin, double workdays,
            int expectedYear, int expectedMonth, int expectedDay, int expectedHour, int expectedMin)
        {
            var startDate = new DateTime(2004, 5, 24, startHour, startMin, 0);
            var expected  = new DateTime(expectedYear, expectedMonth, expectedDay, expectedHour, expectedMin, 0);

            var result = _calculator.CalculateWorkday(startDate, workdays);

            Assert.Equal(expected, result);
        }
    }
}
