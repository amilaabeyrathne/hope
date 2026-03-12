using System.ComponentModel.DataAnnotations;

namespace WorkdayCalendar.API.Models
{
    public class HolidaysRequest
    {
        [Required]
        [MinLength(1)]
        public required List<DateTime> Holidays { get; set; }
    }
}
