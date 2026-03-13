namespace WorkdayCalendar.API.Services
{
    public interface IHolidayRegistryService
    {
        void AddHolidays(IEnumerable<DateOnly> dates);
        void AddRecurringHolidays(IEnumerable<(int Month, int Day)> holidays);
        bool IsWorkday(DateTime date);
    }
}
