using System.ComponentModel.DataAnnotations;
using WorkdayCalendar.API.Models;

namespace WorkdayCalendar.API.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidRecurringHolidayAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not RecurringHoliday holiday)
                return new ValidationResult("Invalid recurring holiday.");

            if (holiday.Month < 1 || holiday.Month > 12)
                return new ValidationResult("Month must be between 1 and 12.", ["Month"]);

            var daysInMonth = DateTime.DaysInMonth(2000, holiday.Month);// took a leap year to ensure February has 29 days
            if (holiday.Day < 1 || holiday.Day > daysInMonth)
                return new ValidationResult($"Day must be between 1 and {daysInMonth} for month {holiday.Month}.", ["Day"]);

            return ValidationResult.Success;
        }
    }
}
