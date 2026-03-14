using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using WorkdayCalendar.API.Validation;

namespace WorkdayCalendar.API.Models
{
    [ValidHolidayDates]
    public class HolidaysRequest
    {
        [Required]
        [MinLength(1)]
        [SwaggerSchema("Array of dates in ISO 8601 format. E.g. [\"2004-05-27T00:00:00\"] or [\"2004-05-27\"]. Must be valid calendar dates.", Nullable = false)]
        public required List<DateTime> Holidays { get; set; }
    }
}
