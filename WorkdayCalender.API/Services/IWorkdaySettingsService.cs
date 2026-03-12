namespace WorkdayCalendar.API.Services
{
    public interface IWorkdaySettingsService
    {
        void SetWorkdayHours(TimeSpan start, TimeSpan stop);
        TimeSpan StartTime { get; }
        TimeSpan StopTime { get; }
    }
}
