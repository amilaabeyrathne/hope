using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using WorkdayCalendar.API.Validation;

namespace WorkdayCalendar.API.Models
{
    public class RecurringHolidaysRequest
    {
        [Required]
        [MinLength(1)]
        [SwaggerSchema("Array of {month, day} objects. Month 1-12, day 1 to max in month. Feb accepts 29. E.g. [{ \"month\": 5, \"day\": 17 }]", Nullable = false)]
        public required List<RecurringHoliday> RecurringHolidays { get; set; }
    }

    [ValidRecurringHoliday]
    [SwaggerSchema(Description = "Month (1-12) and day (1 to max days in month). E.g. month 5, day 17 for 17 May.")]
    public class RecurringHoliday
    {
        [SwaggerSchema("Month (1-12)", Nullable = false)]
        public int Month { get; set; }

        [SwaggerSchema("Day of month (1 to max days; Feb accepts 29)", Nullable = false)]
        public int Day { get; set; }
    }
}
