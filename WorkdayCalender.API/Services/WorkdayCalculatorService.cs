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

        public DateTime CalculateWorkday(DateTime startDate, double workdaysToAdd)
        {
            var direction = workdaysToAdd >= 0 ? 1 : -1;

            if (_workdaySettingsService.StartTime == _workdaySettingsService.StopTime)
                throw new InvalidOperationException("Workday hours have not been configured.");
            
            var workDayMinutes = (_workdaySettingsService.StopTime - _workdaySettingsService.StartTime).TotalMinutes;

            var currentDate = GetValidWorkingWindow(startDate, direction);

            double startProgress = (currentDate.TimeOfDay - _workdaySettingsService.StartTime).TotalMinutes / workDayMinutes;
            double totalWorkdaysFromStart = startProgress + workdaysToAdd;

            int wholeDaysToJump = (int)Math.Floor(totalWorkdaysFromStart);
            double remainingFraction = totalWorkdaysFromStart - wholeDaysToJump;

            for (int i = 0;  i < Math.Abs(wholeDaysToJump); i++)
            {
                currentDate = JumpToNextValidDay(currentDate, wholeDaysToJump >= 0 ? 1 : -1);
            }

            return currentDate.Date
            .Add(_workdaySettingsService.StartTime)
            .AddMinutes(Math.Round(remainingFraction * workDayMinutes));
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
