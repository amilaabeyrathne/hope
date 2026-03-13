namespace WorkdayCalendar.API.Services
{
    public class HolidayRegistryService : IHolidayRegistryService
    {
        private readonly HashSet<DateOnly> _singleHolidays = new();
        private readonly HashSet<(int Month, int Day)> _recurringHolidays = new();

        public void AddHolidays(IEnumerable<DateOnly> dates)
        {
            foreach (var date in dates)
            {
                _singleHolidays.Add(date);
            }
        }

        public void AddRecurringHolidays(IEnumerable<(int Month, int Day)> holidays)
        {
            foreach (var holiday in holidays)
            {
                _recurringHolidays.Add(holiday);
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
