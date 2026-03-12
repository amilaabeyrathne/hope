using System.ComponentModel.DataAnnotations;
using WorkdayCalendar.API.Models;

namespace WorkdayCalendar.API.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndTimeAfterStartTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success; 
            var hours = (WorkdayHoursRequest)value;

            if (hours.End <= hours.Start)
                return new ValidationResult("End time must be greater than start time.", ["End"]);

            return ValidationResult.Success;
        }
    }
}
