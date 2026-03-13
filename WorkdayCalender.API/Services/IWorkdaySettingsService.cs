namespace WorkdayCalendar.API.Services
{
    public interface IWorkdaySettingsService
    {
        void SetWorkdayHours(TimeSpan start, TimeSpan stop);
        TimeSpan StartTime { get; }
        TimeSpan StopTime { get; }
        /// <summary>Work seconds per day (exact, from ticks) for precise fractional workday calculations.</summary>
        decimal WorkSecondsPerDay { get; }
    }
}
