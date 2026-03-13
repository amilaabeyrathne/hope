
namespace WorkdayCalendar.API.Services
{
    public class WorkdaySettingsService : IWorkdaySettingsService
    {
        private TimeSpan _startTime;
        private TimeSpan _stopTime;

        public void SetWorkdayHours(TimeSpan start, TimeSpan stop)
        {
            _startTime = start;
            _stopTime = stop;
        }

        public TimeSpan StartTime => _startTime;

        public TimeSpan StopTime => _stopTime;

        public decimal WorkSecondsPerDay => (decimal)(_stopTime.Ticks - _startTime.Ticks) / TimeSpan.TicksPerSecond;
    }
}
