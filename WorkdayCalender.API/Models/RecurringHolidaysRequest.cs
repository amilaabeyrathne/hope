using System.ComponentModel.DataAnnotations;
using WorkdayCalendar.API.Validation;

namespace WorkdayCalendar.API.Models
{
    public class RecurringHolidaysRequest
    {
        [Required]
        [MinLength(1)]
        public required List<RecurringHoliday> RecurringHolidays { get; set; }
    }

    [ValidRecurringHoliday]
    public class RecurringHoliday
    {
        public int Month { get; set; }
        public int Day { get; set; }
    }
}
