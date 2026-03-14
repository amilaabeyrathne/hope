using System.ComponentModel.DataAnnotations;
using WorkdayCalendar.API.Models;

namespace WorkdayCalendar.API.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidHolidayDatesAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not HolidaysRequest request)
                return ValidationResult.Success;

            if (request.Holidays is null)
                return ValidationResult.Success;

            for (var i = 0; i < request.Holidays.Count; i++)
            {
                var date = request.Holidays[i];
                var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                if (date.Day < 1 || date.Day > daysInMonth)
                    return new ValidationResult(
                        $"'{date:yyyy-MM-dd}' is not a valid date. Day must be between 1 and {daysInMonth} for month {date.Month}.",
                        ["Holidays", i.ToString()]);
            }

            return ValidationResult.Success;
        }
    }
}
