using WorkdayCalendar.API.Models;

namespace WorkdayCalendar.API.Services
{
    public class HolidayRegistryService : IHolidayRegistryService
    {
        private readonly HashSet<DateOnly> _singleHolidays = new();
        private readonly HashSet<(int Month, int Day)> _recurringHolidays = new();

        public void AddHoliday(HolidaysRequest holidaysRequest)
        {
            foreach (var date in holidaysRequest.Holidays)
            {
                _singleHolidays.Add(DateOnly.FromDateTime(date));
            }
        }

        public void AddRecurringHoliday(RecurringHolidaysRequest recurringHolidaysRequest)
        {
            foreach (var holiday in recurringHolidaysRequest.RecurringHolidays)
            {
                _recurringHolidays.Add((holiday.Month, holiday.Day));
            }
        }

        public bool IsWorkday(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return false;

            var day = DateOnly.FromDateTime(date);

            if (_singleHolidays.Contains(day)) return false;

            if (_recurringHolidays.Contains((day.Month, day.Day))) return false;

            return true;
        }
    }
}
