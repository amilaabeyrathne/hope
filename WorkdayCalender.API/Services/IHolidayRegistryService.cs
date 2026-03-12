using WorkdayCalendar.API.Models;

namespace WorkdayCalendar.API.Services
{
    public interface IHolidayRegistryService
    {
        void AddHoliday(HolidaysRequest holidaysRequest);
        void AddRecurringHoliday(RecurringHolidaysRequest recurringHolidaysRequest);
        bool IsWorkday(DateTime date);
    }
}
