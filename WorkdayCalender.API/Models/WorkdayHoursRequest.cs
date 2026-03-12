using System.ComponentModel.DataAnnotations;
using WorkdayCalendar.API.Validation;

namespace WorkdayCalendar.API.Models
{
    [EndTimeAfterStartTime]
    public class WorkdayHoursRequest
    {
        [Required]
        [Range(typeof(TimeSpan), "00:00:00", "23:59:59")]
        public TimeSpan Start { get; set; }

        [Required]
        [Range(typeof(TimeSpan), "00:00:00", "23:59:59")]
        public TimeSpan End { get; set; }
    }
}
