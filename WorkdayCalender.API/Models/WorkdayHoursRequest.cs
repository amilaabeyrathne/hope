using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using WorkdayCalendar.API.Validation;

namespace WorkdayCalendar.API.Models
{
    [EndTimeAfterStartTime]
    public class WorkdayHoursRequest
    {
        [Required]
        [Range(typeof(TimeSpan), "00:00:00", "23:59:59")]
        [SwaggerSchema("Workday start time as HH:mm:ss. E.g. \"08:00:00\"", Nullable = false)]
        public TimeSpan Start { get; set; }

        [Required]
        [Range(typeof(TimeSpan), "00:00:00", "23:59:59")]
        [SwaggerSchema("Workday end time as HH:mm:ss. E.g. \"16:00:00\". Must be after start.", Nullable = false)]
        public TimeSpan End { get; set; }
    }
}
