using System.ComponentModel.DataAnnotations;
using WorkdayCalendar.API.Validation;

namespace WorkdayCalendar.API.Models
{
    [ValidHolidayDates]
    public class HolidaysRequest
    {
        [Required]
        [MinLength(1)]
        public required List<DateTime> Holidays { get; set; }
    }
}
