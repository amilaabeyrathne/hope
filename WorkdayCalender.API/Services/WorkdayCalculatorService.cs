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

            var workSecondsPerDay = _workdaySettingsService.WorkSecondsPerDay;// calculations based on seconds
                                                                              // to avoid issues with fractional workdays and rounding when using TimeSpan for time calculations
            if (workSecondsPerDay <= 0)
                throw new InvalidOperationException("Workday hours have not been configured.");

            var currentDate = GetValidWorkingWindow(startDate, direction);

            var ticksPerSecond = (decimal)TimeSpan.TicksPerSecond;
            var startSecondsIntoDay = (currentDate.TimeOfDay.Ticks - _workdaySettingsService.StartTime.Ticks) / ticksPerSecond;//calculate how many seconds into the workday the start time is, to allow for precise calculations of fractional workdays,
                                                                                                                               //even when the start time is not exactly on a minute boundary.
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
                : Math.Ceiling(secondsSinceMidnight / 60); // when adding workdays, we round down to the nearest minute, when subtracting, we round up,
                                                           // to avoid accidentally jumping to the next day when we are very close to the end of the day. Based on reverse engineering the provided examples,
                                                           // this seems to be the intended behavior.
            var roundedSeconds = minutes * 60;
            var roundedTime = TimeSpan.FromSeconds(roundedSeconds);
            return dt.Date.Add(roundedTime);
        }

        private DateTime GetValidWorkingWindow(DateTime date, int direction)
        {
            int iterations = 0;
            while (!_holidayRegistryService.IsWorkday(date))//if the start date is not a workday, jump to the next workday in the specified direction
            {
                if (iterations++ > MaxWorkdaySearchDays)
                    throw new InvalidOperationException("Too many non-working days in a row. Please check your holiday configuration.");

                date = date.AddDays(direction);
                date = date.Date.Add(direction > 0 ? _workdaySettingsService.StartTime : _workdaySettingsService.StopTime);//when jumping to the next day, we want to start at the beginning of the workday if we are moving forward,
                                                                                                                           //and at the end of the workday if we are moving backward,
                                                                                                                           //to ensure that we don't accidentally skip valid working hours when we are close to the end or start of the day.
            }

            if (date.TimeOfDay < _workdaySettingsService.StartTime)//if we are before the start of the workday, move forward to the start of the workday
            {
                date = date.Date.Add( _workdaySettingsService.StartTime);
            }

            if (date.TimeOfDay > _workdaySettingsService.StopTime)//if we are after the end of the workday, move back to the end of the workday
            {
                date = date.Date.Add( _workdaySettingsService.StopTime);
            }

            return date;
        }


        /// <summary>
        /// Move forward or backward to the next valid workday, skipping non-working days. The time component of the returned date will be the same as the input date.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
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
