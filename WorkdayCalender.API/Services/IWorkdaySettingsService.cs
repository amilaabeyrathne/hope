namespace WorkdayCalendar.API.Services
{
    public interface IWorkdaySettingsService
    {
        void SetWorkdayHours(TimeSpan start, TimeSpan stop);
        TimeSpan StartTime { get; }
        TimeSpan StopTime { get; }
        decimal WorkSecondsPerDay { get; }//Work seconds per day (exact, from ticks) for precise fractional workday calculations.
    }
}
