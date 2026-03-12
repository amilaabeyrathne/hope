namespace WorkdayCalendar.API.Services
{
    public interface IWorkDayCalculatorService
    {
        DateTime CalculateWorkday(DateTime startDate, double workdaysToAdd);
    }
}
