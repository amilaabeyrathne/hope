namespace WorkdayCalendar.API.Services
{
    public class WorkDayCalculatorService : IWorkDayCalculatorService
    {
        private readonly IHolidayRegistryService _holidayRegistryService;
        private readonly IWorkdaySettingsService _workdaySettingsService;
        private const int MaxWorkdaySearchDays = 365; // there cannot be more than 365 workdays in a year, so this is a safe limit to prevent infinite loops
        public WorkDayCalculatorService(IHolidayRegistryService holidayRegistryService, IWorkdaySettingsService workdaySettingsService)
        {
            _holidayRegistryService = holidayRegistryService;
            _workdaySettingsService = workdaySettingsService;
        }

        public DateTime CalculateWorkday(DateTime startDate, decimal workdaysToAdd)
        {
            var direction = workdaysToAdd >= 0 ? 1 : -1;

            var workSecondsPerDay = _workdaySettingsService.WorkSecondsPerDay;
            if (workSecondsPerDay <= 0)
                throw new InvalidOperationException("Workday hours have not been configured.");

            var currentDate = GetValidWorkingWindow(startDate, direction);

            var ticksPerSecond = (decimal)TimeSpan.TicksPerSecond;
            var startSecondsIntoDay = (currentDate.TimeOfDay.Ticks - _workdaySettingsService.StartTime.Ticks) / ticksPerSecond;
            var totalWorkSeconds = startSecondsIntoDay + (workdaysToAdd * workSecondsPerDay);

            var wholeDaysToJump = (int)Math.Floor(totalWorkSeconds / workSecondsPerDay);
            var remainingSeconds = totalWorkSeconds - (wholeDaysToJump * workSecondsPerDay);

            for (var i = 0; i < Math.Abs(wholeDaysToJump); i++)
            {
                currentDate = JumpToNextValidDay(currentDate, wholeDaysToJump >= 0 ? 1 : -1);
            }

            var resultTime = currentDate.Date
                .Add(_workdaySettingsService.StartTime)
                .AddSeconds((double)remainingSeconds);

            return RoundToNearestMinute(resultTime, workdaysToAdd >= 0);
        }

        private static DateTime RoundToNearestMinute(DateTime dt, bool addWorkdays)
        {
            var secondsSinceMidnight = dt.TimeOfDay.TotalSeconds;
            var minutes = addWorkdays
                ? Math.Floor(secondsSinceMidnight / 60)
                : Math.Ceiling(secondsSinceMidnight / 60);
            var roundedSeconds = minutes * 60;
            var roundedTime = TimeSpan.FromSeconds(roundedSeconds);
            return dt.Date.Add(roundedTime);
        }

        private DateTime GetValidWorkingWindow(DateTime date, int direction)
        {
            int iterations = 0;
            while (!_holidayRegistryService.IsWorkday(date))
            {
                if (iterations++ > MaxWorkdaySearchDays)
                    throw new InvalidOperationException("Too many non-working days in a row. Please check your holiday configuration.");

                date = date.AddDays(direction);
                date = date.Date.Add(direction > 0 ? _workdaySettingsService.StartTime : _workdaySettingsService.StopTime);
            }

            if (date.TimeOfDay < _workdaySettingsService.StartTime)
            {
                date = date.Date.Add( _workdaySettingsService.StartTime);
            }

            if (date.TimeOfDay > _workdaySettingsService.StopTime)
            {
                date = date.Date.Add( _workdaySettingsService.StopTime);
            }

            return date;
        }
       
        private DateTime JumpToNextValidDay(DateTime current, int direction) 
        {
            int iterations = 0;

            do  {
                current = current.AddDays(direction);

                if (iterations++ > MaxWorkdaySearchDays)
                    throw new InvalidOperationException("Too many non-working days in a row. Please check your holiday configuration.");

            } while (!_holidayRegistryService.IsWorkday(current));

            return current;
        }
    }
}
